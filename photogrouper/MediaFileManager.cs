using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Managers
{
    public class MediaFileManager : IMediaFileManager
    {
        /// <summary>
        /// Logger Control.
        /// </summary>
        private ILogger _logger;

        public MediaFileManager(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async IFileCollection GetFiles(string path)
        {
            return await Task.Run(() => GetFilesSync(path));
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<TagLib.File> GetFilesSync(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Directory does not exist");

            var fileNames = Directory.GetFiles(path);

            List<TagLib.File> files = new List<TagLib.File>();

            foreach (string file in fileNames)
            {
                TagLib.File tagFile = null;
                try
                {
                    tagFile = TagLib.File.Create(file);
                }
                catch (NotImplementedException e)
                {
                    _logger.WriteLine("Not Implemented: {0}", file);
                }
                catch (TagLib.UnsupportedFormatException e)
                {
                    _logger.WriteLine("Unsupported File: {0}", file);
                    //ConsoleHelper.ConsoleLineBreak();
                    continue;
                }

                // Raw image files
                var tiffFile = tagFile as TagLib.Tiff.File;
                if (tiffFile != null)
                {
                    files.Add(tiffFile);
                    _logger.WriteTabbed(file, tiffFile.ImageTag.Exif.DateTime);
                    continue;
                }

                // Get the image type.
                var imageFile = tagFile as TagLib.Image.File;
                if (imageFile != null)
                {
                    files.Add(imageFile);
                    _logger.WriteTabbed(file, imageFile.ImageTag.Exif.DateTime);
                    continue;
                }

                // Mp4 Files
                var mp4File = tagFile as TagLib.Mpeg4.File;
                if (mp4File != null)
                {
                    files.Add(mp4File);
                    _logger.WriteTabbed(file, mp4File.Tag.DateTagged);
                    continue;
                }

                _logger.WriteLine("Unsupported file: {0}", file);

            }
            return files;
        }

        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> GetFilesJson(string path)
        {
            return JsonConvert.SerializeObject(await GetFiles(path), new JsonSerializerSettings() { ContractResolver = new TagLibFileContractResolver() });
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetFilesJsonSync(string path)
        {
            return JsonConvert.SerializeObject(GetFilesSync(path), new JsonSerializerSettings() { ContractResolver = new TagLibFileContractResolver() });
        }
    }

    public interface IMediaFileManager
    {
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IEnumerable<TagLib.File>> GetFiles(string directory);
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<TagLib.File> GetFilesSync(string directory);
        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<string> GetFilesJson(string path);
        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetFilesJsonSync(string path);
    }
}

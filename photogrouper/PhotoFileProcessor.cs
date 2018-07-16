using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper
{
    public class PhotoFileProcessor : IPhotoFileProcessor
    {
        /// <summary>
        /// Logger Control.
        /// </summary>
        private ILogger _logger;

        public PhotoFileProcessor(ILogger logger)
        {
            this._logger = logger;
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<IPhotoCollection> GetFiles(string path)
        {
            return await GetFiles(path, false, null);
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IPhotoCollection GetFilesSync(string path, bool recursive)
        {
            return GetFiles(path, recursive, null).Result;
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IPhotoCollection GetFilesSync(string path)
        {
            return GetFilesSync(path, false);
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<IPhotoCollection> GetFiles(string path, bool recursive, IProgress<int> update)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Directory does not exist");

            List<PhotoFile> files = new List<PhotoFile>();

            var enumerator = Directory.EnumerateFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            int index = 0;
            int count = enumerator.Count();
            foreach (string file in enumerator)
            {
                TagLib.File tagFile = null;
                try
                {
                    index++;
                    update?.Report(index * 100 / count);

                    tagFile = await GetTagLibfile(file, tagFile);
                }
                catch (NotImplementedException)
                {
                    await _logger.WriteLine("ERROR: Not Implemented: {0}", file);
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    await _logger.WriteLine("ERROR: Unsupported File: {0}", file);
                    continue;
                }
                catch (TagLib.CorruptFileException)
                {
                    await _logger.WriteLine("ERROR: File Corrupt: {0}", file);
                    continue;
                }

                //await _logger.WriteLine(file);
                // Raw image files
                var tiffFile = tagFile as TagLib.Tiff.File;
                if (tiffFile != null)
                {
                    files.Add(ProcessTagFile(tiffFile, file));
                    continue;
                }

                // Get the image type.
                var imageFile = tagFile as TagLib.Image.File;
                if (imageFile != null)
                {
                    files.Add(ProcessTagFile(imageFile, file));
                    continue;
                }

                // Mp4 Files
                var mp4File = tagFile as TagLib.Mpeg4.File;
                if (mp4File != null)
                {
                    files.Add(ProcessTagFile(mp4File, file));
                    continue;
                }

                await _logger.WriteLine("ERROR: Unsupported file: {0}", file);
            }

            return new PhotoCollection(files, path);
        }
        /// <summary>
        /// Gets the taglib file asynchronously.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tagFile"></param>
        /// <returns></returns>
        private static async Task<TagLib.File> GetTagLibfile(string file, TagLib.File tagFile) => await Task.Run(() => TagLib.File.Create(file) );

        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> GetFilesJson(string path)
        {
            return JsonConvert.SerializeObject(await GetFiles(path));//, new JsonSerializerSettings() { ContractResolver = new FileContractResolver() });
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetFilesJsonSync(string path)
        {
            return JsonConvert.SerializeObject(GetFilesSync(path));// new JsonSerializerSettings() { ContractResolver = new TagLibFileContractResolver() });
        }

        #region Private Helper Methods

        /// <summary>
        /// Implements TagLib.Image.File and TagLib.Tiff.File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private PhotoFile ProcessTagFile(TagLib.Image.File file, string fullPath)
        {
            var exif = file.ImageTag.Exif;
            DateTime createdDate = File.GetCreationTime(fullPath);
            DateTime date = (exif != null ? (exif.DateTimeOriginal ?? exif.DateTime) : null) ?? file.ImageTag.DateTime ?? createdDate;

            return new PhotoFile(fullPath, date, createdDate);
        }
        /// <summary>
        /// Processes an MP4 file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private PhotoFile ProcessTagFile(TagLib.Mpeg4.File file, string fullPath)
        {
            DateTime createdDate = File.GetCreationTime(fullPath);

            return new PhotoFile(fullPath, createdDate, createdDate);
        }

        #endregion
    }

    public interface IPhotoFileProcessor
    {
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IPhotoCollection> GetFiles(string directory);
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IPhotoCollection> GetFiles(string directory, bool recursive, IProgress<int> update);
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IPhotoCollection GetFilesSync(string directory);
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IPhotoCollection GetFilesSync(string directory, bool recursive);
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

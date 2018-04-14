using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Managers
{
    public class FileManager
    {
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<TagLib.File>> GetFiles(string directory)
        {
            return await Task.Run(() => GetFilesSync(directory));
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<TagLib.File> GetFilesSync(string directory)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException("Directory does not exist");

            var fileNames = Directory.GetFiles(directory);

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
                    Logger.Writer.WriteLine("Not Implemented: {0}", file);
                }
                catch (TagLib.UnsupportedFormatException e)
                {
                    Logger.Writer.WriteLine("Unsupported File: {0}", file);
                    //ConsoleHelper.ConsoleLineBreak();
                    continue;
                }

                var image = tagFile as TagLib.Image.File;
                var tiff = tagFile as TagLib.Tiff.File;

                if (image == null && tiff == null)
                {
                    Logger.Writer.WriteLine("Not an Image File: {0}", file);
                    //ConsoleHelper.ConsoleLineBreak();
                    continue;
                }
                // Add the tag lib file to the collection to return.
                files.Add(tagFile);

                Logger.WriteTabbed(file
                    , image.ImageTag.Exif.DateTimeDigitized
                    , image.ImageTag.Exif.DateTimeOriginal
                    , image.ImageTag.Exif.DateTime);

            }
            return files;
        }

        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async static Task<string> GetFilesJson(string path)
        {
            return JsonConvert.SerializeObject(await GetFiles(path), new JsonSerializerSettings() { ContractResolver = new TagLibFileContractResolver() });
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path in JSON Format.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFilesJsonSync(string path)
        {
            return JsonConvert.SerializeObject(GetFilesSync(path), new JsonSerializerSettings() { ContractResolver = new TagLibFileContractResolver() });
        }
    }
}

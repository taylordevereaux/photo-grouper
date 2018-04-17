﻿using Newtonsoft.Json;
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
            return await Task.Run(() => GetFilesSync(path));
        }
        /// <summary>
        /// Returns all the TagLib Files for the file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IPhotoCollection GetFilesSync(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Directory does not exist");

            var fileNames = Directory.GetFiles(path);

            List<PhotoFile> files = new List<PhotoFile>();

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
                    files.Add(ProcessTagFile(tiffFile, file));
                    _logger.WriteTabbed(file, tiffFile.ImageTag.Exif.DateTime);
                    continue;
                }

                // Get the image type.
                var imageFile = tagFile as TagLib.Image.File;
                if (imageFile != null)
                {
                    files.Add(ProcessTagFile(imageFile, file));
                    _logger.WriteTabbed(file, imageFile.ImageTag.Exif.DateTime);
                    continue;
                }

                // Mp4 Files
                var mp4File = tagFile as TagLib.Mpeg4.File;
                if (mp4File != null)
                {
                    files.Add(ProcessTagFile(mp4File, file));
                    _logger.WriteTabbed(file, mp4File.Tag.DateTagged);
                    continue;
                }

                _logger.WriteLine("Unsupported file: {0}", file);
            }

            return new PhotoCollection(files);
        }

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
            DateTime date = exif.DateTimeOriginal ?? exif.DateTime ?? file.ImageTag.DateTime ?? createdDate;

            return new PhotoFile(fullPath, date, createdDate, file);
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

            return new PhotoFile(fullPath, createdDate, createdDate, file);
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
        IPhotoCollection GetFilesSync(string directory);
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
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper
{
    public class PhotoCollection : IPhotoCollection
    {
        private List<PhotoFile> _list = new List<PhotoFile>();

        private string _directory;

        public PhotoCollection(List<PhotoFile> files, string directory)
        {
            this._list.AddRange(files);
            this._directory = directory;
        }

        /// <summary>
        /// Groups Each Photo into a folder based on the specific text value.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IPhotoCollection> GroupBy(Func<PhotoFile, string> predicate)
        {
            /// The new list of photo files we'll use to create a new photo collection to return from.
            List<PhotoFile> newFiles = new List<PhotoFile>();

            // Invalid folder characters used to validate the new group by folder names.
            char[] invalidCharacters = Path.GetInvalidPathChars();

            foreach (var file in this._list)
            {
                // Overflow check
                checked
                {
                    string groupBy = predicate(file);
                    // We need to validate the groupBy folder contains no invalid characters.
                    if (groupBy.ToCharArray().Count(x => invalidCharacters.Contains(x)) > 0)
                        throw new InvalidOperationException(String.Format("Group by folder contains invalid characters: {0}", groupBy));

                    string target = Path.Combine(_directory, groupBy);

                    string fileName = file.FilePath;

                    string newFileName = Path.Combine(target, Path.GetFileName(fileName));

                    if (!File.Exists(fileName))
                        throw new Exception(String.Format("File no longer exists: {0}", fileName));

                    // If the file paths are going to be the same we don't need to do anything.
                    if (Path.Equals(fileName, newFileName))
                    {
                        newFiles.Add(new PhotoFile(newFileName, file.Date, file.DateCreated));
                        continue;
                    }

                    if (!Directory.Exists(target))
                        Directory.CreateDirectory(target);

                    // Move the file.
                    await MoveFileAsync(fileName, newFileName);

                    newFiles.Add(new PhotoFile(newFileName, file.Date, File.GetCreationTime(newFileName)));

                    DeleteFileAndEmptyDirectory(fileName);
                }
            }
            /// Clear and recreate ourselfs.
            return ClearAndCreate(newFiles);
        }

        /// <summary>
        /// UnGroups Each Photo from their current folder to the root folder.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IPhotoCollection> UnGroup()
        {
            /// The new list of photo files we'll use to create a new photo collection to return from.
            List<PhotoFile> newFiles = new List<PhotoFile>();

            // Invalid folder characters used to validate the new group by folder names.
            char[] invalidCharacters = Path.GetInvalidPathChars();

            foreach (var file in this._list)
            {
                // Existing File Name
                string fileName = file.FilePath;
                // Filename in new directory.
                string newFileName = Path.Combine(_directory, Path.GetFileName(fileName));

                if (!File.Exists(fileName))
                    throw new Exception(String.Format("File no longer exists: {0}", fileName));
                // If the file paths are going to be the same we don't need to do anything.
                if (Path.Equals(fileName, newFileName))
                {
                    newFiles.Add(new PhotoFile(newFileName, file.Date, file.DateCreated));
                    continue;
                }
                // Move the file.
                await MoveFileAsync(fileName, newFileName);
                // Add the new file with updated file path.
                newFiles.Add(new PhotoFile(newFileName, file.Date, File.GetCreationTime(newFileName)));
                // Deletes
                DeleteFileAndEmptyDirectory(fileName);
            }
            /// Clear and recreate ourselfs.
            return ClearAndCreate(newFiles);
        }

        /// <summary>
        /// The count of photo files.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this._list.Count;
        }

        #region File Helper Methods
        /// <summary>
        /// Moves the file from one location to another.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        private static async Task MoveFileAsync(string fileName, string newFileName)
        {
            using (FileStream sourceStream = File.Open(fileName, FileMode.Open))
            {
                using (FileStream destinationStream = File.Create(newFileName))
                {
                    // Create the new file.
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }
        /// <summary>
        /// Clears out the current object and returns a new one.
        /// </summary>
        /// <param name="newFiles"></param>
        /// <returns></returns>
        private IPhotoCollection ClearAndCreate(List<PhotoFile> newFiles)
        {
            // Clear out the existing photo collection.
            _list.Clear();
            // Tmeporary directory we'll clear out this class to return a new one.
            string directory = _directory;
            _directory = "";
            return new PhotoCollection(newFiles, directory);
        }

        /// <summary>
        /// Delets the file and attempts to delete its directory if the directory is empty and the directory is not our root directory.
        /// </summary>
        /// <param name="fileName"></param>
        private void DeleteFileAndEmptyDirectory(string fileName)
        {
            // Delete old File.
            File.Delete(fileName);

            // If the Directory is now empty we'll delete that folder as well.
            string fileDirectory = Path.GetDirectoryName(fileName);
            if (!Path.Equals(fileDirectory, _directory) && Directory.GetFileSystemEntries(fileDirectory).Count() == 0)
                Directory.Delete(fileDirectory);
        }
        #endregion

        #region Json
        public async Task<string> ToJson()
        {
            return await Task.Run(() => ToJsonSync());
        }

        public string ToJsonSync()
        {
            return JsonConvert.SerializeObject(this._list);
        }
        #endregion

        #region Enumerator

        public IEnumerator<PhotoFile> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }

    public interface IPhotoCollection : IEnumerable<PhotoFile>
    {
        Task<string> ToJson();

        string ToJsonSync();

        Task<IPhotoCollection> GroupBy(Func<PhotoFile, string> predicate);

        Task<IPhotoCollection> UnGroup();

        int Count();
    }
}

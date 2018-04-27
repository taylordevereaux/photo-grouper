using Newtonsoft.Json;
using PhotoGrouper.Exceptions;
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
        public IPhotoCollection GroupBy(Func<PhotoFile, string> predicate)
        {
            // Invalid folder characters used to validate the new group by folder names.
            char[] invalidCharacters = Path.GetInvalidPathChars();

            return MoveFiles(file => {

                string groupBy = predicate(file);
                // We need to validate the groupBy folder contains no invalid characters.
                if (groupBy.ToCharArray().Count(x => invalidCharacters.Contains(x)) > 0)
                    throw new InvalidOperationException(String.Format("Group by folder format includes invalid characters: {0}", groupBy));

                string target = Path.Combine(_directory, groupBy);

                string fileName = file.FilePath;

                return Path.Combine(target, Path.GetFileName(fileName));
            });
        }

        /// <summary>
        /// UnGroups Each Photo from their current folder to the root folder.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IPhotoCollection UnGroup()
        {
            return MoveFiles(file => Path.Combine(_directory, Path.GetFileName(file.FilePath)));
        }

        /// <summary>
        /// Moves the files to a new folder.
        /// </summary>
        /// <returns></returns>
        public IPhotoCollection Move(string directory)
        {
            string oldDirectory = this._directory;
            this._directory = directory;
            return MoveFiles(file => Path.Combine(_directory, file.FilePath.Replace(oldDirectory, "")));
        }

        /// <summary>
        /// Groups Each Photo into a folder based on the specific text value.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IPhotoCollection> Confirm()
        {
            return await Confirm(null);
        }
        /// <summary>
        /// Groups Each Photo into a folder based on the specific text value.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IPhotoCollection> Confirm(IProgress<int> update)
        {
            /// The new list of photo files we'll use to create a new photo collection to return from.
            List<PhotoFile> newFiles = new List<PhotoFile>();

            int index = 1;
            int count = this._list.Count;
            foreach (var file in this._list)
            {
                // Overflow check
                checked
                {
                    // After moving the file we need to send an update.
                    update?.Report(index * 100 / count);
                    index++;

                    string oldFileName = file.OldFilePath;
                    string newFileName = file.FilePath;

                    string target = Path.GetDirectoryName(newFileName);

                    if (!File.Exists(oldFileName))
                        throw new FileDoesNotExistException(String.Format("File no longer exists: {0}", oldFileName));

                    // If the file paths are going to be the same we don't need to do anything.
                    if (Path.Equals(oldFileName, newFileName))
                    {
                        newFiles.Add(new PhotoFile(newFileName, file.Date, file.DateCreated));
                        continue;
                    }

                    if (!Directory.Exists(target))
                        Directory.CreateDirectory(target);

                    // Move the file.
                    await MoveFileAsync(oldFileName, newFileName);

                    newFiles.Add(new PhotoFile(newFileName, file.Date, File.GetCreationTime(newFileName)));

                    DeleteFileAndEmptyDirectory(oldFileName);

                    System.Threading.Thread.Sleep(50);

                }
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
        /// Renames the files based on the getFileName predicate.
        /// </summary>
        /// <param name="getNewPath"></param>
        /// <returns></returns>
        private IPhotoCollection MoveFiles(Func<PhotoFile, string> getNewPath)
        {
            /// The new list of photo files we'll use to create a new photo collection to return from.
            List<PhotoFile> newFiles = new List<PhotoFile>();

            foreach (var file in this._list)
            {
                // Validate the file still exists.
                if (!File.Exists(file.FilePath))
                    throw new FileDoesNotExistException(String.Format("File no longer exists: {0}", file.FilePath));

                string newFilePath = getNewPath(file);

                // Validate the new file doesn't already exist.
                if (!Path.Equals(file.FilePath, newFilePath) && File.Exists(newFilePath))
                    throw new FileExistsException(String.Format("File already exists: {0}", newFilePath));

                // Add the new file with updated file path.
                newFiles.Add(new PhotoFile(newFilePath, file.Date, file.DateCreated, file.FilePath));
            }
            /// Clear and recreate ourselfs.
            return ClearAndCreate(newFiles);
        }
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

        IPhotoCollection GroupBy(Func<PhotoFile, string> predicate);

        IPhotoCollection UnGroup();

        IPhotoCollection Move(string directory);

        Task<IPhotoCollection> Confirm();

        Task<IPhotoCollection> Confirm(IProgress<int> update);

        int Count();
    }
}

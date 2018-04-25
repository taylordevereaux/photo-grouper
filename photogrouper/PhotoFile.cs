using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace PhotoGrouper
{
    /// <summary>
    /// Defines the file 
    /// </summary>
    public class PhotoFile
    {
        /// <summary>
        /// Creates a new Media File Instance.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="date"></param>
        /// <param name="dateCreated"></param>
        /// <param name="file"></param>
        public PhotoFile(string fileName, DateTime date, DateTime dateCreated)
        {
            FilePath = fileName;
            Date = date;
            DateCreated = dateCreated;
            //File = file;
        }
        /// <summary>
        /// Creates a new Media File Instance.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="date"></param>
        /// <param name="dateCreated"></param>
        /// <param name="file"></param>
        public PhotoFile(string fileName, DateTime date, DateTime dateCreated, string oldFilePath)
            : this(fileName, date, dateCreated)
        {
            this.OldFilePath = oldFilePath;
        }

        /// <summary>
        /// The FileName of the file.
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// The Main Date Associated with the file. (DateTaken, Media Date Created)
        /// </summary>
        public DateTime Date { get; private set; }
        /// <summary>
        /// The Created Date of the file.
        /// </summary>
        public DateTime DateCreated { get; private set; }
        /// <summary>
        /// The New FileName of the file.
        /// </summary>
        public string OldFilePath { get; private set; }


    }
}

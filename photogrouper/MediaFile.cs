using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace PhotoGrouper.Managers
{
    /// <summary>
    /// Defines the file 
    /// </summary>
    public class MediaFile
    {
        /// <summary>
        /// Creates a new Media File Instance.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="date"></param>
        /// <param name="dateCreated"></param>
        /// <param name="file"></param>
        public MediaFile(string fileName, DateTime date, DateTime dateCreated, File file)
        {
            FileName = fileName;
            Date = date;
            DateCreated = dateCreated;
            File = file;
        }

        /// <summary>
        /// The FileName of the file.
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// The Main Date Associated with the file. (DateTaken, Media Date Created)
        /// </summary>
        public DateTime Date { get; private set; }
        /// <summary>
        /// The Created Date of the file.
        /// </summary>
        public DateTime DateCreated { get; private set; }
        /// <summary>
        /// The Associated TagLib file that was created.
        /// </summary>
        public TagLib.File File { get; private set; }

        
    }
}

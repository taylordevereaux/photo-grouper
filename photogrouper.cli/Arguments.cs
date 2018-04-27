using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Cli
{
    internal class CommandArguments
    {
        /// <summary>
        /// The command to execute agains.
        /// </summary>
        public string Command = "";
        /// <summary>
        /// Directories to search.
        /// </summary>
        public string Directory = "";
        /// <summary>
        /// Format to group by
        /// </summary>
        public string Format = "";
        /// <summary>
        /// Recursively search past child directories.
        /// </summary>
        public bool Recursive = false;
        /// <summary>
        /// Return the results in a json format.
        /// </summary>
        public bool Json = false;
        /// <summary>
        /// Log the results to the console or file.
        /// </summary>
        public bool Log = false;
        /// <summary>
        /// Show help
        /// </summary>
        public bool Help = false;
        /// <summary>
        /// Prompt before continueing.
        /// </summary>
        public bool ConfirmPrompt = true;

        public CommandArguments()
        {
        }

        public CommandArguments(CommandArguments copy)
        {
            this.Directory = copy.Directory;
            this.Format = copy.Format;
            this.Recursive = copy.Recursive;
            this.Json = copy.Json;
            this.Log = copy.Log;
            this.ConfirmPrompt = copy.ConfirmPrompt;
        }
    }
}

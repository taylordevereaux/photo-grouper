using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mono.Options;
using PhotoGrouper.Managers;

namespace PhotoGrouper.Cli
{

    class Arguments
    {
        public string Directory = "";

        public bool Group = false;

        public bool UnGroup = false;

        public string DateFormat = "";

        public bool Recursive = false;

        public bool Json = false;

        public bool Log = false;
    }

    public class Program
    {

        public static async Task Main(string[] args)
        {
            var arguments = new Arguments();
            bool showHelp = false;

            var options = new OptionSet() {
                // The directory to sort in.
                { "d|directory=", "the directory to list", d => arguments.Directory = d },
                { "g|group", "group the photos into folder", g => arguments.Group = g != null },
                { "u|ungroup", "ungroup the photos in folders", g => arguments.UnGroup = g != null },
                { "j|json", "return the json result", j => arguments.Json = j != null },
                { "l|log", "include logs in the results", l => arguments.Log = l != null },
                { "r|recursive", "recursively search all subfolders for files", g => arguments.Recursive = g != null },
                { "f|dateformat", "format to group the photos by (Default = yyyy-MM-dd)", a => arguments.DateFormat = a },
                { "h|help=", "show this message and exit", h => showHelp = h != null }
            };

            if (args.Length == 0)
            {
                while (args.Length == 0)
                {
                    Console.Write("photogrouper: ");
                    // Get the arguments.
                    string line = Console.ReadLine();
                    args = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);    

                    if (args.Length > 0)
                    {
                        // If arguments are passed directly to the application we will parse them right away.
                        ParseArguments(args, options);
                        // Execute the arguments.
                        await Execute(arguments, showHelp, options);

                        args = new string[0];
                    }
                }
            } 
            else if (args.Length > 0)
            {
                // If arguments are passed directly to the application we will parse them right away.
                ParseArguments(args, options);
                // Execute the arguments.
                await Execute(arguments, showHelp, options);
            }

        }

        /// <summary>
        /// Executes the application based on the arguments passed.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="showHelp"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static async Task Execute(Arguments arguments, bool showHelp, OptionSet options)
        {
            // Display the help if we need to
            if (showHelp)
            {
                ShowHelp(options);
            }
            // Validate the directory 
            else if (ValidateArguments(arguments))
            {
                SetDefaultArguments(arguments);
                
                var processor = new PhotoFileProcessor((arguments.Log ? (ILogger)new Logger() : new EmptyLogger()));

                var files = await processor.GetFiles(arguments.Directory, arguments.Recursive);

                if (arguments.Group)
                    files = await files.GroupBy(x => x.Date.ToString(arguments.DateFormat));
                else if (arguments.UnGroup)
                    files = await files.UnGroup();
                //files = await files.UnGroup();
                if (arguments.Json)
                    Console.WriteLine(await files.ToJson());
            }
        }

        /// <summary>
        /// Sets any default arguments that may not be set.
        /// </summary>
        /// <param name="arguments"></param>
        private static void SetDefaultArguments(Arguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.DateFormat))
                arguments.DateFormat = "yyyy-MM-dd";
        }

        /// <summary>
        /// Parses the arguments passed to the console application.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="options"></param>
        private static void ParseArguments(string[] args, OptionSet options)
        {
            RemoveExtraQuotes(args);

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("photogrouper: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try 'photogrouper --help' for more information.");
            }
        }
        /// <summary>
        /// Removes extra quotes from arguments passed.
        /// </summary>
        /// <param name="args"></param>
        private static void RemoveExtraQuotes(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Contains("\""))
                    args[i] = args[i].Replace("\"", "");
            }
        }

        private static bool ValidateArguments(Arguments arguments)
        {
            // The directory is required.
            if (string.IsNullOrWhiteSpace(arguments.Directory))
            {
                Console.WriteLine("<directory> is required");
                return false;
            }
            // The directory needs to exist.
            if (!Directory.Exists(arguments.Directory))
            {
                Console.WriteLine("<directory> does not exist");
                return false;
            }
            return true;
        }

        private static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Help description in progress");
            options.WriteOptionDescriptions(Console.Out);
        }
    }

    internal class EmptyLogger : ILogger
    {
        public void LineBreak()
        {
            //throw new NotImplementedException();
        }

        public void Write(string message)
        {
            //throw new NotImplementedException();
        }

        public void WriteLine(string message)
        {
            //throw new NotImplementedException();
        }

        public void WriteLine(string message, params object[] arg)
        {
            //throw new NotImplementedException();
        }

        public void WriteTabbed(params object[] arg)
        {
            //throw new NotImplementedException();
        }
    }
}
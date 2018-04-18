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
                { "h|help=", "show this message and exit", h => showHelp = h != null }
            };

            if (args.Length > 0)
            {
                try
                {
                    options.Parse(args);
                }
                catch (OptionException e)
                {
                    Console.Write("file-sorter: ");
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Try 'file-sorter --help' for more information.");
                }
            }
            else
            {
#if DEBUG
                arguments.Directory = "C:\\Users\\tdevereaux\\Pictures\\Temp";
#else
                do
                {
                    Console.Write("Directory: ");
                    arguments.Directory = Console.ReadLine();
                }
                while (!ValidateArguments(arguments));
#endif
            }
            // Display the help if we need to
            if (showHelp)
            {
                ShowHelp(options);
            }
            // Validate the directory 
            else if (ValidateArguments(arguments))
            {
                //var files = PhotoFileProcessor.GetFilesSync(arguments.Directory);

                var processor = new PhotoFileProcessor(new Logger());

                var files = await processor.GetFiles(arguments.Directory, true);

                files = await files.GroupBy(x => x.Date.Date.ToString("yyyy-MM-dd"));

                //files = await files.UnGroup();

                Console.WriteLine(await files.ToJson());
            }

            Console.ReadKey();
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
}
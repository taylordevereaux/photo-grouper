using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using PhotoGrouper.Cli.Loggers;
using PhotoGrouper.Managers;

namespace PhotoGrouper.Cli
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var parser = new ArgumentParser();

            if (args.Length == 0)
            {
                Console.WriteLine("Welcome to Photo Grouper! Try `--help` for a list of commands.");
                while (args.Length == 0)
                {
                    Console.Write("$ ");
                    // Get the arguments.
                    string line = Console.ReadLine();
                    args = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);    

                    if (args.Length > 0)
                    {
                        try
                        {
                            var result = parser.Parse(args);
                            // Because we are using the console to execute commands we want to prompt prior to making changes.
                            result.ConfirmPrompt = true;
                            // Execute the arguments.
                            await Execute(result);
                        }
                        catch (Exception e)
                        {
                            Console.Write("ERROR: " + e.Message);
                        }
                        args = new string[0];
                    }
                }
            } 
            else if (args.Length > 0)
            {
                var result = parser.Parse(args);
                // Execute the arguments.
                await Execute(result);
            }

        }


        /// <summary>
        /// Executes the application based on the arguments passed.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="showHelp"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static async Task Execute(Arguments arguments)
        {
            if (!String.IsNullOrEmpty(arguments.Command))
            {
                var executer = new CommandExecuter(
                    new PhotoFileProcessor(
                        (arguments.Log ? new TextLogger() : arguments.ConfirmPrompt ? (ILogger)new Logger() : new EmptyLogger())
                    )
                );

                await executer.Execute(arguments);
            }
        }
    }

    internal class Arguments
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
        public bool ConfirmPrompt = false;
    }

    internal class ArgumentParser
    {
        public Arguments Parse(string[] args)
        {
            var _ = ProgramHelpCommand.HELP_TEXT;

            var arguments = new Arguments();

            void setDirectory(string a) => arguments.Directory = a;
            void setFormat(string a) => arguments.Format = a;
            void setRecursive(string a) => arguments.Recursive = a != null;
            void setJson(string a) => arguments.Json = a != null;
            void setLog(string a) => arguments.Log = a != null;

            // Help Command
            void ShowCommandHelp(Command command)
            {
                Console.WriteLine("\nUsage:\n  photogrouper {0}  {1}\n\nOptions:", command.Name, command.Help);
                command.Options.WriteOptionDescriptions(Console.Out);
                // We need to reset the command to now continue after displaying the help text.
                arguments.Command = "";
            }
            // Executes the run command.
            void RunCommand(OptionSet options, IEnumerable<string> commandArgs, string command)
            {
                arguments.Command = command.ToLower();
                try
                {
                    options.Parse(commandArgs);
                }
                catch (OptionException e)
                {
                    Console.Write("photogrouper: ");
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Try 'photogrouper --help' for more information.");
                }

                SetDefaultArguments(arguments);
            }
            // Create the Commands
            var groupCommand = new Command("group", _["group"]);
            var ungroupCommand = new Command("ungroup", _["ungroup"]);
            var listCommand = new Command("list", _["list"]);

            // Create Command Options
            var groupOptions = new OptionSet()
            {
                { "d|directory=", _["directory"], setDirectory },
                { "r|recursive", _["recursive"], setRecursive },
                { "f|format=", _["format"], setFormat },
                { "l|log", _["log"], setLog },
                { "h|help", _["help"], (y) => { ShowCommandHelp(groupCommand); } }
            };
            var ungroupOptions = new OptionSet()
            {
                { "d|directory=", _["directory"], setDirectory },
                { "r|recursive", _["recursive"], setRecursive },
                { "l|log", _["log"], setLog },
                { "h|help", _["help"], (y) => { ShowCommandHelp(ungroupCommand); } }
            };
            var listOptions = new OptionSet()
            {
                { "d|directory=", _["directory"], setDirectory },
                { "r|recursive", _["recursive"], setRecursive },
                { "j|json", _["json"], setJson },
                { "l|log", _["log"], setLog },
                { "h|help", _["help"], (y) => { ShowCommandHelp(listCommand); } }
            };

            // Update Command Options
            groupCommand.Options = groupOptions;
            groupCommand.Run = commandArgs => RunCommand(groupOptions, commandArgs, "group");
            ungroupCommand.Options = ungroupOptions;
            ungroupCommand.Run = commandArgs => RunCommand(ungroupOptions, commandArgs, "ungroup");
            listCommand.Options = listOptions;
            listCommand.Run = commandArgs => RunCommand(listOptions, commandArgs, "list");

            var suite = new CommandSet("photogrouper");
            suite.Add(new ProgramHelpCommand());
            suite.Add(groupCommand);
            suite.Add(ungroupCommand);
            suite.Add(listCommand);
            //suite.Add(new HelpCommand());

            RemoveExtraQuotes(args);

            suite.Run(args);

            return arguments;
        }


        #region Arguments Parser

        /// <summary>
        /// Sets any default arguments that may not be set.
        /// </summary>
        /// <param name="arguments"></param>
        private void SetDefaultArguments(Arguments arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments.Directory))
                arguments.Directory = Directory.GetCurrentDirectory();
            if (string.IsNullOrWhiteSpace(arguments.Format))
                arguments.Format = "yyyy-MM-dd";
        }
        /// <summary>
        /// Removes extra quotes from arguments passed.
        /// </summary>
        /// <param name="args"></param>
        private void RemoveExtraQuotes(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Contains("\""))
                    args[i] = args[i].Replace("\"", "");
            }
        }

        #endregion
    }

    internal class CommandExecuter
    {

        private IPhotoFileProcessor _processor;

        public CommandExecuter(IPhotoFileProcessor processor)
        {
            this._processor = processor;
        }

        public async Task Execute(Arguments arguments)
        {
            //var processor = new PhotoFileProcessor((arguments.Log ? new TextLogger() : promptToConfirm ? (ILogger)new Logger() : new EmptyLogger()));
            var files = await _processor.GetFiles(arguments.Directory, arguments.Recursive);

            if (arguments.Command == "group")
            {
                if (!arguments.ConfirmPrompt || ConfirmGroup(files, arguments))
                    files = await files.GroupBy(x => x.Date.ToString(arguments.Format));
            }
            else if (arguments.Command == "ungroup")
            {
                if (!arguments.ConfirmPrompt || ConfirmUnGroup(files, arguments))
                    files = await files.UnGroup();
            }
            else if (arguments.Command == "list")
            {
                //files = await files.UnGroup();
                if (arguments.Json)
                    Console.WriteLine(await files.ToJson());
                else
                {
                    StringBuilder br = new StringBuilder();
                    foreach (var file in files)
                        br.AppendLine(file.FilePath);
                    Console.WriteLine(br.ToString());
                }
            }
        }
        #region Confirm Prompts
        /// <summary>
        /// Confirm the grouping based on the arguments passed.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private bool ConfirmUnGroup(IPhotoCollection files, Arguments arguments)
        {

            Console.WriteLine("Ungroup {0} files into directory {1} from sub-directories and removing any empty sub-directories?"
                , files.Count()
                , arguments.Directory);
            return ConfirmYes();
        }

        /// <summary>
        /// Confirm the grouping based on the arguments passed.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private bool ConfirmGroup(IPhotoCollection files, Arguments arguments)
        {
            Console.WriteLine("Group {0} files from directory {1}{2} into folders by date taken, formated as '{3}'?"
                , files.Count()
                , arguments.Directory
                , arguments.Recursive ? " and sub-directories" : ""
                , arguments.Format);
            return ConfirmYes();
        }

        /// <summary>
        /// Reads the input from the user and gets yes or no input.
        /// </summary>
        /// <returns></returns>
        private bool ConfirmYes()
        {
            Console.Write("[Y] Yes (default), [N] No: ");
            string key = Console.ReadLine();
            return key.StartsWith("Y") || key.StartsWith("y");
        }
        #endregion
    }
    /// <summary>
    /// Inherites the HelpCommand to override default functionality.
    /// </summary>
    internal class ProgramHelpCommand : HelpCommand
    {
        internal static readonly Dictionary<string, string> HELP_TEXT = new Dictionary<string, string>() {
            { "group", "[--directory|-d] <directory> [--format|-f] <format> [--recursive|-r] [--log|-l]" },
            { "ungroup", "[--directory|-d] <directory> [--recursive|-r] [--log|-l]" },
            { "list", "[--directory|-d] <directory> [--recursive|-r]  [--log|-l] [--json|-j]" },
            {"help", "Show this and exit." },
            {"version", "Show version." },
            {"directory", "Directory to execute against [default: current directory]." },
            {"format", "Specify the date format for folder grouping [default: yyyy-MM-dd]." },
            {"recursive", "Includes photos in all sub-directories [default: false]." },
            {"json", "Returns the list results in json format." },
            {"log", "Logs output to the console window." }
        };

        private void ShowCommandHelp(Command command)
        {
            Console.WriteLine("\nUsage:\n  photogrouper {0}  {1}\n\nOptions:", command.Name, command.Help);
            command.Options.WriteOptionDescriptions(Console.Out);
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            var extra = new List<string>(arguments ?? new string[0]);

            if (extra.Count > 0)
            {
                var command = CommandSet.GetCommand(extra);
                if (command != null)
                {
                    ShowCommandHelp(command);
                    return 0;
                }
                else 
                {
                    WriteUnknownCommand(extra[0]);
                    return 1;
                }
            }
            else
            {
                var _ = HELP_TEXT;
                Console.WriteLine(
$@"Usage:
  photogrouper group {_["group"]}
  photogrouper ungroup {_["group"]}
  photogrouper list {_["group"]}
  photogrouper -h | --help
  photogrouper --version

Options:
  -h --help       {_["help"]}
  --version       {_["version"]}
  -d --directory  {_["directory"]}
  -f --format     {_["format"]}
  -r --recursive  {_["recursive"]}
  -j --json       {_["json"]}
  -l --log        {_["log"]}");
                return 0;
            }
        }
    }
}
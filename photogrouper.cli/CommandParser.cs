using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Cli
{
    internal class CommandParser
    {

        /// <summary>
        /// Parses the argument input into commands.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<CommandArguments> Parse(string[] args)
        {
            List<CommandArguments> commands = new List<CommandArguments>();
            CommandArguments firstCommand = null;
            foreach (string[] arg in args.Split(x => x == "|"))
            {
                CommandArguments command = firstCommand = ParseCommandArguments(args, firstCommand == null, firstCommand);
                commands.Add(command);
            }
            return commands;
        }

        private CommandArguments ParseCommandArguments(string[] args, bool first, CommandArguments firstCommand)
        {
            var _ = HelpCommand.HELP_TEXT;

            // If we are passed a first command we want to copy most of its settings to our new command.
            var arguments = firstCommand != null ? new CommandArguments(firstCommand) : new CommandArguments();

            void setDirectory(string a) => arguments.Directory = a;
            void setFormat(string a) => arguments.Format = a;
            void setRecursive(string a) => arguments.Recursive = a != null;
            void setJson(string a) => arguments.Json = a != null;
            void setLog(string a) => arguments.Log = a != null;

            // Help Command
            void ShowCommandHelp(Mono.Options.Command command)
            {
                Console.WriteLine("\nUsage:\n  photogrouper {0}  {1}\n\nOptions:", command.Name, command.Help);
                command.Options.WriteOptionDescriptions(Console.Out);
                // We need to reset the command to now continue after displaying the help text.
                arguments.Command = "";
            }
            // Executes the run command.
            void RunCommand(Mono.Options.OptionSet options, IEnumerable<string> commandArgs, string command)
            {
                arguments.Command = command.ToLower();
                try
                {
                    options.Parse(commandArgs);
                }
                catch (Mono.Options.OptionException e)
                {
                    Console.Write("photogrouper: ");
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Try 'photogrouper --help' for more information.");
                }

                SetDefaultArguments(arguments);
            }
            // Create the Commands
            var groupCommand = new Mono.Options.Command("group", _["group"]);
            var ungroupCommand = new Mono.Options.Command("ungroup", _["ungroup"]);
            var listCommand = new Mono.Options.Command("list", _["list"]);

            // Create Command Options
            var groupOptions = first ? new Mono.Options.OptionSet()
            {
                { "d|directory=", _["directory"], setDirectory },
                { "r|recursive", _["recursive"], setRecursive },
                { "f|format=", _["format"], setFormat },
                { "l|log", _["log"], setLog },
                { "h|help", _["help"], (y) => { ShowCommandHelp(groupCommand); } }
            }
            : new Mono.Options.OptionSet()
            {
                { "f|format=", _["format"], setFormat }
            };
            var ungroupOptions = first ? new Mono.Options.OptionSet()
            {
                { "d|directory=", _["directory"], setDirectory },
                { "r|recursive", _["recursive"], setRecursive },
                { "l|log", _["log"], setLog },
                { "h|help", _["help"], (y) => { ShowCommandHelp(ungroupCommand); } }
            } : new Mono.Options.OptionSet();

            var listOptions = first ? new Mono.Options.OptionSet()
            {
                { "d|directory=", _["directory"], setDirectory },
                { "r|recursive", _["recursive"], setRecursive },
                { "j|json", _["json"], setJson },
                { "l|log", _["log"], setLog },
                { "h|help", _["help"], (y) => { ShowCommandHelp(listCommand); } }
            }
            : new Mono.Options.OptionSet()
            { 
                { "r|recursive", _["recursive"], setRecursive },
                { "j|json", _["json"], setJson }
            };

            // Update Command Options
            groupCommand.Options = groupOptions;
            groupCommand.Run = commandArgs => RunCommand(groupOptions, commandArgs, "group");
            ungroupCommand.Options = ungroupOptions;
            ungroupCommand.Run = commandArgs => RunCommand(ungroupOptions, commandArgs, "ungroup");
            listCommand.Options = listOptions;
            listCommand.Run = commandArgs => RunCommand(listOptions, commandArgs, "list");

            var suite = new Mono.Options.CommandSet("photogrouper");
            suite.Add(new HelpCommand());
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
        private void SetDefaultArguments(CommandArguments arguments)
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
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using PhotoGrouper.Cli.Loggers;

namespace PhotoGrouper.Cli
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var parser = new CommandParser();

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
                            var results = parser.Parse(args);
                            // Because we are using the console to execute commands we want to prompt prior to making changes.
                            //result.ConfirmPrompt = true;
                            // Execute the arguments.
                            await Execute(results);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ERROR: " + e.Message);
                        }
                        args = new string[0];
                    }
                }
            } 
            else if (args.Length > 0)
            {
                var results = parser.Parse(args);
                // Execute the arguments.
                await Execute(results);
            }

        }


        /// <summary>
        /// Executes the application based on the arguments passed.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="showHelp"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static async Task Execute(IEnumerable<CommandArguments> arguments)
        {
            var list = new List<CommandArguments>(arguments);
            // We just exit if there is no commands or the user asks for help opton.
            if (arguments.Where(x => !String.IsNullOrEmpty(x.Command) && !x.Help).Count() == arguments.Count() && arguments.Count() > 0)
            {
                // Logging and prompting determined by first command.
                var argument = arguments.First();

                var executer = new CommandExecuter(
                    new PhotoFileProcessor(
                        (argument.Log ? new TextLogger() : argument.ConfirmPrompt ? (ILogger)new Logger() : new EmptyLogger())
                    )
                );

                await executer.Execute(arguments);
            }
        }
    }
}
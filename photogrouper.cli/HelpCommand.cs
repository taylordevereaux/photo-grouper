using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Cli
{
    /// <summary>
    /// Inherites the HelpCommand to override default functionality.
    /// </summary>
    internal class HelpCommand : Mono.Options.HelpCommand
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

        private void ShowCommandHelp(Mono.Options.Command command)
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

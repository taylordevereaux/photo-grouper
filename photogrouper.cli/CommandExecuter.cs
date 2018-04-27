using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Cli
{
    internal class CommandExecuter
    {

        private IPhotoFileProcessor _processor;

        public CommandExecuter(IPhotoFileProcessor processor)
        {
            this._processor = processor;
        }

        public async Task Execute(IEnumerable<CommandArguments> arguments)
        {
            var argument = arguments.First();
            //var processor = new PhotoFileProcessor((arguments.Log ? new TextLogger() : promptToConfirm ? (ILogger)new Logger() : new EmptyLogger()));
            var files = await _processor.GetFiles(argument.Directory, argument.Recursive);

            foreach (var a in arguments)
                files = await Execute(a, files);
        }

        private async Task<IPhotoCollection> Execute(CommandArguments arguments, IPhotoCollection files)
        {
            //var processor = new PhotoFileProcessor((arguments.Log ? new TextLogger() : promptToConfirm ? (ILogger)new Logger() : new EmptyLogger()));
            //var files = await _processor.GetFiles(arguments.Directory, arguments.Recursive);
            if (arguments.Command == "group")
            {
                files = await HandleConfirm(files.GroupBy(x => x.Date.ToString(arguments.Format)), arguments);
            }
            else if (arguments.Command == "ungroup")
            {
                files = await HandleConfirm(files.UnGroup(), arguments);
            }
            else if (arguments.Command == "list")
            {
                await ListFiles(files, arguments);
            }

            return files;
        }

        #region Commands

        /// <summary>
        /// Lists the files out to the user.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        private async Task ListFiles(IPhotoCollection files, CommandArguments arguments)
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

        #endregion

        #region Confirm Prompts
        /// <summary>
        /// Confirm the grouping based on the arguments passed.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private async Task<IPhotoCollection> HandleConfirm(IPhotoCollection files, CommandArguments arguments)
        {

            IProgress<int> updateProgress = new Progress<int>(percentage =>
            {
                Console.Write("\rProgress: {0}%", percentage);
            });

            int fileCount = files.Count();

            if (!arguments.ConfirmPrompt || await ConfirmPrompt(files, arguments))
            {
                Console.Write("Progress: 0%");

                files = await files.Confirm(updateProgress);

                Console.WriteLine("\nSuccessfully moved {0} files.", fileCount);
            }

            return files;
        }

        /// <summary>
        /// Confirm the grouping based on the arguments passed.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private async Task<bool> ConfirmPrompt(IPhotoCollection files, CommandArguments arguments)
        {
            await ListFiles(files, arguments);

            Console.WriteLine("Confirm moving {0} files?"
                , files.Count());

            Console.Write("[Y] Yes (default), [N] No: ");

            string key = Console.ReadLine();
            return key.StartsWith("Y") || key.StartsWith("y");
        }

        #endregion
    }
}

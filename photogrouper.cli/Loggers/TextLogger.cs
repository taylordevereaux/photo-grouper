using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Cli.Loggers
{
    public class TextLogger : ILogger
    {
        private string _logFileName = "";

        public TextLogger()
        {
            this._logFileName = Path.Combine(Directory.GetCurrentDirectory(), String.Format("log.{0}.txt", DateTime.Now.ToString("yyyyMMddhhmm")));
            //File.Create(_logFileName).Dispose();
        }

        public async Task LineBreak()
        {
            await InternalWrite("------------------------------------");
        }

        public async Task Write(string message)
        {
            await InternalWrite(message, false);
        }

        public async Task WriteLine(string message)
        {
            await InternalWrite(message);
        }

        public async Task WriteLine(string message, params object[] arg)
        {
            await InternalWrite(String.Format(message, arg));
        }

        public async Task WriteTabbed(params object[] arg)
        { 
            await InternalWrite(String.Join("\t", arg));
        }
        /// <summary>
        /// Handles the writting to log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="writeLine"></param>
        /// <returns></returns>
        private async Task InternalWrite(string message, bool writeLine = true)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_logFileName, true))
                {
                    await writer.WriteLineAsync(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: [Log] - {0}", e.Message);
            }
        }
    }
}

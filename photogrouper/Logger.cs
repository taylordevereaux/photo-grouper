using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper
{
    public class Logger : ILogger
    {
        /// <summary>
        /// Writes all the parameters separated by a tab.
        /// </summary>
        /// <param name="arg"></param>
        public Task WriteTabbed(params object[] arg)
        {
            return Task.Run(() => Console.WriteLine(string.Join("\t", arg)));
        }
        /// <summary>
        /// Displays a new Line followed by a line, then by a final new line.
        /// </summary>
        public Task LineBreak()
        {
            return Task.Run(() =>
            {
                Console.WriteLine();
                Console.WriteLine("----------------------------------");
                Console.WriteLine();
            });
        }
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        public Task WriteLine(string message)
        {
            return Task.Run(() => Console.WriteLine(message));
        }
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        public Task WriteLine(string message, params object[] arg)
        {
            return Task.Run(() => Console.WriteLine(message, arg));
        }
        /// <summary>
        /// Replicates Console.Write
        /// </summary>
        /// <param name="message"></param>
        public Task Write(string message)
        {
            return Task.Run(() => Console.WriteLine(message) );
        }
    }

    public interface ILogger
    {

        /// <summary>
        /// Writes all the parameters separated by a tab.
        /// </summary>
        /// <param name="arg"></param>
        Task WriteTabbed(params object[] arg);
        /// <summary>
        /// Displays a new Line followed by a line, then by a final new line.
        /// </summary>
        Task LineBreak();
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        Task WriteLine(string message);
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        Task WriteLine(string message, params object[] arg);
        /// <summary>
        /// Replicates Console.Write
        /// </summary>
        /// <param name="message"></param>
        Task Write(string message);
    }
}

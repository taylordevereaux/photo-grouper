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
        public void WriteTabbed(params object[] arg)
        {
            Console.WriteLine(string.Join("\t", arg));
        }
        /// <summary>
        /// Displays a new Line followed by a line, then by a final new line.
        /// </summary>
        public void LineBreak()
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------");
            Console.WriteLine();
        }
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message, params object[] arg)
        {
            Console.WriteLine(message, arg);
        }
        /// <summary>
        /// Replicates Console.Write
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public interface ILogger
    {

        /// <summary>
        /// Writes all the parameters separated by a tab.
        /// </summary>
        /// <param name="arg"></param>
        void WriteTabbed(params object[] arg);
        /// <summary>
        /// Displays a new Line followed by a line, then by a final new line.
        /// </summary>
        void LineBreak();
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        void WriteLine(string message);
        /// <summary>
        /// Replicates Console.WriteLine
        /// </summary>
        /// <param name="message"></param>
        void WriteLine(string message, params object[] arg);
        /// <summary>
        /// Replicates Console.Write
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);
    }
}

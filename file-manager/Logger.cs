using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Managers
{
    public class Logger 
    {
        /// <summary>
        /// The interface to output logging information to.
        /// </summary>
        private static TextWriter _logger = Console.Out;

        /// <summary>
        /// Registers the Default Logger as a Console.Out
        /// </summary>
        //internal static void RegisterDefaultLogger()
        //{
        //    if (_logger == null)
        //        _logger = Console.Out;
        //}

        /// <summary>
        /// Register the TextWriter to output logs to.
        /// </summary>
        /// <param name="writer"></param>
        public static void RegisterLogger(TextWriter writer)
        {
            _logger = writer;
        }
        /// <summary>
        /// REturns the TextWriter control.
        /// </summary>
        /// <returns></returns>
        public static TextWriter Writer { get { return _logger; } }

        /// <summary>
        /// Displays a new Line followed by a line, then by a final new line.
        /// </summary>
        public static void LineBreak()
        {
            _logger.WriteLine();
            _logger.WriteLine("----------------------------------");
            _logger.WriteLine();
        }

        /// <summary>
        /// Writes all the parameters separated by a tab.
        /// </summary>
        /// <param name="arg"></param>
        public static void WriteTabbed(params object[] arg)
        {
            _logger.WriteLine(string.Join("\t", arg));
        }
    }
}

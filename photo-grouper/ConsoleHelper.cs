using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper
{
    public static class ConsoleHelper
    {
        /// <summary>
        /// Displays a new Line followed by a line, then by a final new line.
        /// </summary>
        public static void ConsoleLineBreak()
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------");
            Console.WriteLine();
        }

        /// <summary>
        /// Writes all the parameters separated by a tab.
        /// </summary>
        /// <param name="arg"></param>
        public static void WriteTabbed(params object[] arg)
        {
            Console.WriteLine(string.Join("\t", arg));
        }
    }
}

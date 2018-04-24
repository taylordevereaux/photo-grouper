using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Cli.Loggers
{
    internal class EmptyLogger : ILogger
    {
        public Task LineBreak()
        {
            return Task.Delay(0); //throw new NotImplementedException();
        }

        public Task Write(string message)
        {
            return Task.Delay(0); //throw new NotImplementedException();
        }

        public Task WriteLine(string message)
        {
            return Task.Delay(0); //throw new NotImplementedException();
        }

        public Task WriteLine(string message, params object[] arg)
        {
            return Task.Delay(0); //throw new NotImplementedException();
        }

        public Task WriteTabbed(params object[] arg)
        {
            return Task.Delay(0); //throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Exceptions
{
    public class FileExistsException : Exception
    {
        public FileExistsException()
        {
        }

        public FileExistsException(string message) : base(message)
        {
        }

        public FileExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class FileDoesNotExistException : Exception
    {
        public FileDoesNotExistException()
        {
        }

        public FileDoesNotExistException(string message) : base(message)
        {
        }

        public FileDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

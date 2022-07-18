using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAPI.Exceptions
{
    public class DatabaseUninitializedException : Exception
    {
        public DatabaseUninitializedException()
        {
        }

        public DatabaseUninitializedException(string? message) : base(message)
        {
        }

        public DatabaseUninitializedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DatabaseUninitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

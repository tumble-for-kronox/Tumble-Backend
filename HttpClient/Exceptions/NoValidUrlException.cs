using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TumbleHttpClient.Exceptions
{
    public class NoValidUrlException : Exception
    {
        public NoValidUrlException()
        {
        }

        public NoValidUrlException(string? message) : base(message)
        {
        }

        public NoValidUrlException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoValidUrlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

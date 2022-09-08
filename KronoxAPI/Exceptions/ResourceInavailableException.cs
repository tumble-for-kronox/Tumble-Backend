using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Exceptions;

public class ResourceInavailableException : Exception
{
    public ResourceInavailableException()
    {
    }

    public ResourceInavailableException(string? message) : base(message)
    {
    }

    public ResourceInavailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ResourceInavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

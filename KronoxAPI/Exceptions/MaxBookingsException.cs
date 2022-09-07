using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Exceptions;

public class MaxBookingsException : Exception
{
    public MaxBookingsException()
    {
    }

    public MaxBookingsException(string? message) : base(message)
    {
    }

    public MaxBookingsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected MaxBookingsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

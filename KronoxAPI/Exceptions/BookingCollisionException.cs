using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Exceptions;

public class BookingCollisionException : Exception
{
    public BookingCollisionException()
    {
    }

    public BookingCollisionException(string? message) : base(message)
    {
    }

    public BookingCollisionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BookingCollisionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

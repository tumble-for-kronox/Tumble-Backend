using System.Runtime.Serialization;

namespace KronoxBackend.Exceptions
{
    public class InvalidSchoolException : Exception
    {
        public InvalidSchoolException()
        {
        }

        public InvalidSchoolException(string? message) : base(message)
        {
        }

        public InvalidSchoolException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidSchoolException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

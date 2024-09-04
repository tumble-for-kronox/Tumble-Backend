using System.Runtime.Serialization;

namespace TumbleBackend.Exceptions
{
    public class TranslatorUninitializedException : Exception
    {
        public TranslatorUninitializedException()
        {
        }

        public TranslatorUninitializedException(string? message) : base(message)
        {
        }

        public TranslatorUninitializedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TranslatorUninitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace HLHML.LanguageElements
{
    [Serializable]
    internal class InvalidNodeNumberException : Exception
    {
        public InvalidNodeNumberException()
        {
        }

        public InvalidNodeNumberException(string message) : base(message)
        {
        }

        public InvalidNodeNumberException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidNodeNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
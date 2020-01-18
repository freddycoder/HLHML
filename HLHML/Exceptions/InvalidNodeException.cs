using System;
using System.Runtime.Serialization;

namespace HLHML.LanguageElements
{
    [Serializable]
    internal class InvalidNodeException : Exception
    {
        public InvalidNodeException()
        {
        }

        public InvalidNodeException(string message) : base(message)
        {
        }

        public InvalidNodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
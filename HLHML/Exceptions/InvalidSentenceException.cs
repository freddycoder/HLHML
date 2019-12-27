using System;
using System.Runtime.Serialization;

namespace HLHML
{
    [Serializable]
    internal class InvalidSentenceException : Exception
    {
        public InvalidSentenceException()
        {
        }

        public InvalidSentenceException(string message) : base(message)
        {
        }

        public InvalidSentenceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSentenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
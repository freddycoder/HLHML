using System;
using System.Runtime.Serialization;

namespace HLHML
{
    [Serializable]
    internal class InvalidScopeException : Exception
    {
        public InvalidScopeException()
        {
        }

        public InvalidScopeException(string message) : base(message)
        {
        }

        public InvalidScopeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidScopeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
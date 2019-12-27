using System;
using System.Runtime.Serialization;

namespace HLHML
{
    [Serializable]
    internal class ScopeNotInitializeException : Exception
    {
        public ScopeNotInitializeException()
        {
        }

        public ScopeNotInitializeException(string message) : base(message)
        {
        }

        public ScopeNotInitializeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScopeNotInitializeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
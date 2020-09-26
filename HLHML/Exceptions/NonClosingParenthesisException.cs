using System;
using System.Runtime.Serialization;

namespace HLHML.Exceptions
{
    [Serializable]
    internal class NonClosingParenthesisException : Exception
    {
        public NonClosingParenthesisException()
        {
        }

        public NonClosingParenthesisException(string message) : base(message)
        {
        }

        public NonClosingParenthesisException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NonClosingParenthesisException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
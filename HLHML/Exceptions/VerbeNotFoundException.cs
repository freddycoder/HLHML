using System;
using System.Runtime.Serialization;

namespace HLHML
{
    [Serializable]
    internal class VerbeNotFoundException : Exception
    {
        private Token? token;

        public VerbeNotFoundException()
        {
        }

        public VerbeNotFoundException(Token token)
        {
            this.token = token;
        }

        public VerbeNotFoundException(string message) : base(message)
        {
        }

        public VerbeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VerbeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
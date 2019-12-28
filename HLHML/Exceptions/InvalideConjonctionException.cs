using System;
using System.Runtime.Serialization;

namespace HLHML.LanguageElements
{
    [Serializable]
    internal class InvalideConjonctionException : Exception
    {
        public InvalideConjonctionException()
        {
        }

        public InvalideConjonctionException(string message) : base(message)
        {
        }

        public InvalideConjonctionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalideConjonctionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace HLHML.LanguageElements
{
    [Serializable]
    internal class InvalidPredicatException : Exception
    {
        public InvalidPredicatException()
        {
        }

        public InvalidPredicatException(string message) : base(message)
        {
        }

        public InvalidPredicatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPredicatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;

namespace HLHML.Exceptions
{
    public class ParseurException : Exception
    {
        public ParseurException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}

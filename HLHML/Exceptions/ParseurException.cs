using HLHML.AnalyseurLexical;
using System;

namespace HLHML.Exceptions
{
    public class ParseurException : Exception
    {
        public ParseurException(ILexer lexer, string message, Exception? innerException = null) : base(message, innerException)
        {
            Lexer = lexer;
        }

        public ILexer Lexer { get; }
    }
}

using System;

namespace HLHML.Exceptions
{
    public class NonClosingQuoteException : Exception
    {
        /// <param name="message">Message de l'exception</param>
        /// <param name="beginAt">L'indice du début du guillemet double</param>
        /// <param name="text">Le text qui contient le guillement double</param>
        public NonClosingQuoteException(string? message, int beginAt, string text) : base(message)
        {
            BeginAt = beginAt;
            Text = text;
        }

        public int BeginAt { get; }
        public string Text { get; }
    }
}

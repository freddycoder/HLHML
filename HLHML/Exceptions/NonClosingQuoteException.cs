using System;
using System.Collections.Generic;
using System.Text;

namespace HLHML.Exceptions
{
    public class NonClosingQuoteException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
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

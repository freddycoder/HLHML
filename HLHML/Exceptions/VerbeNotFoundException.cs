using System;
using System.Runtime.Serialization;
using HLHML.Dictionnaire;

namespace HLHML.Exceptions
{
    [Serializable]
    internal class VerbeNotFoundException : Exception
    {
        private Terme? terme;

        public VerbeNotFoundException()
        {
        }

        public VerbeNotFoundException(Terme terme)
        {
            this.terme = terme;
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
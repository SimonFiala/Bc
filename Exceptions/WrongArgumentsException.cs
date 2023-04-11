using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bc.Exceptions
{
    public class WrongArgumentsException : Exception
    {
        public WrongArgumentsException()
        {
        }

        public WrongArgumentsException(string message) : base(message)
        {
        }

        public WrongArgumentsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongArgumentsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace QDMSServer.ViewModels
{
    [Serializable]
    internal class CommandAbortException : Exception
    {
        public CommandAbortException()
        {
        }

        public CommandAbortException(string message) : base(message)
        {
        }

        public CommandAbortException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandAbortException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
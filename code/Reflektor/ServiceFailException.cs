using System;

namespace Reflektor
{
    public class ServiceFailException : Exception
    {
        public ServiceFailException(string message) : base(message)
        {
        }

        public ServiceFailException(Exception innerException) : base(innerException.Message, innerException)
        {
        }

        public ServiceFailException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

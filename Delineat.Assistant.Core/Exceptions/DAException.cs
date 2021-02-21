using System;

namespace Delineat.Assistant.Core.Exceptions
{
    public class DAException: Exception
    {
        public DAException(string message) : base(message)
        {
        }

        public DAException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

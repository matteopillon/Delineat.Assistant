using Delineat.Assistant.Core.Exceptions;
using System;

namespace Delineat.Assistant.Core.Stores.Exceptions
{
    public class DAStoresException : DAException
    {
        public DAStoresException(string message) : base(message)
        {
        }

        public DAStoresException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

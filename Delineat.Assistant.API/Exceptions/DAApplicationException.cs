using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Exceptions
{
    public class DAApplicationException : Exception
    {
        public DAApplicationException(string message) : base(message)
        {
        }

        public DAApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
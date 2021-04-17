using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.LocalService
{
    public class ProcessResult
    {

        public ProcessResult(bool isOk, string message = "")
        {
            IsOK = isOk;
            Message = message;
        }

        public bool IsOK { get; }
        public string Message { get; }
    }
}

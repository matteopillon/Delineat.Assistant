using Delineat.Assistant.Models;
using System;
using System.Collections.Generic;

namespace Delineat.Assistant.API.Models.Results
{
    public class DAUploadResult
    {
        public DAUploadResult()
        {
            NeedPasswordFiles = new List<string>();
        }
        public string FileName { get; set; }
        public Guid LoadingSessionId { get; set; }
        public DWTips Tips { get; set; }

        public List<string> NeedPasswordFiles { get; set; }
    }
}

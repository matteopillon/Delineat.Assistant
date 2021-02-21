using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWUpload
    {
        public DWUpload()
        {
            LoadingSessionId = Guid.Empty;
            FileName = string.Empty;
            Tips = new DWTips();
        }

        public string FileName { get; set; }
        public Guid LoadingSessionId { get; set; }
        public DWTips Tips { get; set; }

        public List<string> NeedPasswordFiles { get; set; }
    }
}

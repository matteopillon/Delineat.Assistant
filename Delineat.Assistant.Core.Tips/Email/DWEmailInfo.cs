using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Tips.Email
{
    public class DWEmailInfo
    {
        private readonly List<string> to = new List<string>();
        private readonly List<string> cc = new List<string>();
        private readonly List<string> bcc = new List<string>();
        private readonly List<DWEmailAttachment> attachments = new List<DWEmailAttachment>();

        public string From { get; set; }
        public DateTime? DeliverDateUtc { get; set; }
        public string Subject { get; set; }
        public List<string> To { get => to; }
        public List<string> CC { get => cc; }
        public List<string> BCC { get => bcc; }

        public List<DWEmailAttachment> Attachments { get => attachments; }
    }

    public class DWEmailAttachment
    {
        public string Filename { get; set; }
        public byte[] Data { get; set; }
    }
}

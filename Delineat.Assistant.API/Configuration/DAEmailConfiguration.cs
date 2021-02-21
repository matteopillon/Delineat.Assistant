using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Configuration
{
    public class DAEmailConfiguration
    {
        public bool EnableSsl { get; set; }

        public string EmailPassword { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SmtpServer { get; set; }
        public string EmailUsername { get; set; }
        public string[] InternalEmails { get; set; }
    }
}

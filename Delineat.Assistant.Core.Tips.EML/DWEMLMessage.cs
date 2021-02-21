using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Tips.EML
{
    public class DWEMLMessage
    {
        private List<DWEMLAttachment> attachments;

        public DWEMLMessage(string emlFilePath)
        {

            var message = DWCDOWrapper.LoadMessage(emlFilePath);
            this.From = message.From;
            if (message.ReceivedTime > message.SentOn)
                this.DeliverDate = message.ReceivedTime;
            else
                this.DeliverDate = message.SentOn;

            if (this.DeliverDate.Year < 1970)
            {
                try
                {
                    var field = message.Fields["urn:schemas:mailheader:ÿþdate"] as ADODB.Field;
                    if (field != null && field.Value != null)
                    {
                        DateTime date;

                        if (DateTime.TryParse(field.Value.ToString(), out date))
                        {
                            this.DeliverDate = date;
                        }
                    }
                }
                catch
                {

                }
            }


            this.To = (message.To ?? string.Empty).Split(';');
            this.CC = (message.CC ?? string.Empty).Split(';');
            this.BCC = (message.BCC ?? string.Empty).Split(';');
            this.Subject = message.Subject;

            this.TextBody = message.TextBody;
            this.HtmlBody = message.HTMLBody;
            this.attachments = new List<DWEMLAttachment>();
            foreach (var attachment in message.Attachments)
            {
                var bodyPartAttachment = attachment as CDO.IBodyPart;
                if (bodyPartAttachment != null)
                {
                    var emlAttachment = new DWEMLAttachment();
                    emlAttachment.Filename = bodyPartAttachment.FileName;
                    emlAttachment.Index = attachments.Count + 1;
                    emlAttachment.Data = bodyPartAttachment.ToByteArray();
                    attachments.Add(emlAttachment);
                }
            }
        }



        public string Subject { get; private set; }
        public string TextBody { get; private set; }
        public string HtmlBody { get; private set; }

        public List<DWEMLAttachment> Attachments
        {
            get
            {
                return attachments;
            }
        }

        public string From { get; private set; }
        public DateTime DeliverDate { get; }
        public string[] To { get; private set; }
        public string[] BCC { get; private set; }
        public string[] CC { get; private set; }
    }
}

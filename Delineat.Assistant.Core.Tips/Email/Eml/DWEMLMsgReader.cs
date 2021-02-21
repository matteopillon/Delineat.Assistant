using Delineat.Assistant.Core.Tips.EML;
using System.Collections.Generic;
using System.IO;

namespace Delineat.Assistant.Core.Tips.Email.EML
{
    public class DWEMLMsgReader : IDWEmailReader
    {




        public DWEmailInfo GetEmailInfo(object obj)
        {

            try
            {
                var path = obj as string;
                if (path != null)
                {
                    switch (Path.GetExtension(path).ToLower())
                    {
                        case ".eml":
                            DWEmailInfo emailInfo = new DWEmailInfo();
                            var emlMessage = new DWEMLMessage(path);
                            {
                                emailInfo.Subject = emlMessage.Subject;
                                emailInfo.From = emlMessage.From;
                                emailInfo.DeliverDateUtc = emlMessage.DeliverDate;
                                AddRecipients(emailInfo.To, emlMessage.To);
                                AddRecipients(emailInfo.CC, emlMessage.CC);
                                AddRecipients(emailInfo.BCC, emlMessage.BCC);

                                if (emlMessage.Attachments != null)
                                {
                                    foreach (var emailAttachment in emlMessage.Attachments)
                                    {
                                        DWEmailAttachment attachment = new DWEmailAttachment();
                                        attachment.Filename = $"Allegato {emlMessage.Attachments.IndexOf(emailAttachment)}";
                                        if (!string.IsNullOrWhiteSpace(emailAttachment.Filename))
                                            attachment.Filename = emailAttachment.Filename;

                                        attachment.Data = emailAttachment.Data;
                                        emailInfo.Attachments.Add(attachment);

                                    }
                                }
                            }
                            return emailInfo;
                    }
                }
            }
            catch
            {

            }
            return null;
        }


        private void AddRecipients(List<string> whos, string[] recipients)
        {
            foreach (var recipient in recipients)
            {
                if (!whos.Contains(recipient))
                {
                    whos.Add(recipient);
                }

            }
        }
    }
}

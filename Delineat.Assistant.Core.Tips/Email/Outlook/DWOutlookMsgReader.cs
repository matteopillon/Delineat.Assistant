using Delineat.Assistant.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Delineat.Assistant.Core.Tips.Email.Outlook.DWOutlookStorage;

namespace Delineat.Assistant.Core.Tips.Email.Outlook
{
    public class DWOutlookMsgReader : IDWEmailReader
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
                        case ".msg":
                            DWEmailInfo emailInfo = new DWEmailInfo();
                            using (var outlookMessage = new DWOutlookStorage.Message(path))
                            {
                                FillEmailInfoFromMessage(emailInfo, outlookMessage);
                                ExportOutlookMessage(emailInfo, outlookMessage);
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

        private void FillEmailInfoFromMessage(DWEmailInfo emailInfo, Message outlookMessage)
        {
            emailInfo.From = outlookMessage.From;

            if (outlookMessage.DeliverDateUtc.HasValue)
                emailInfo.DeliverDateUtc = outlookMessage.DeliverDateUtc.Value.ToLocalTime();
            emailInfo.Subject = outlookMessage.Subject;

            AddRecipients(emailInfo.To, outlookMessage, RecipientType.To);
            AddRecipients(emailInfo.CC, outlookMessage, RecipientType.CC);
        }

        private void ExportOutlookMessage(DWEmailInfo emailInfo, Message outlookMessage)
        {

            if (outlookMessage.Messages != null)
            {
                foreach (var attachedMessage in outlookMessage.Messages)
                {
                    DWEmailAttachment attachment = new DWEmailAttachment();
                    attachment.Filename = DAFileHelper.GetSafeFilename(Path.ChangeExtension(attachedMessage.Subject, "msg"));
                    using (var ms = new MemoryStream())
                    {
                        attachedMessage.Save(ms);
                        attachment.Data = ms.ToArray();
                        try
                        {
                            ms.Position = 0;
                            using (var outlookSubMessage = new DWOutlookStorage.Message(ms))
                            {

                                ExportOutlookMessage(emailInfo, outlookSubMessage);
                            }
                        }
                        catch
                        {

                        }
                    }
                    emailInfo.Attachments.Add(attachment);


                }
            }



            if (outlookMessage.Attachments != null)
            {
                foreach (var outlookAttachment in outlookMessage.Attachments)
                {
                    try
                    {
                        DWEmailAttachment attachment = new DWEmailAttachment();

                        attachment.Filename = $"Allegato {outlookMessage.Attachments.IndexOf(outlookAttachment)}";
                        if (!string.IsNullOrWhiteSpace(outlookAttachment.Filename))
                            attachment.Filename = outlookAttachment.Filename;
                        else if (!string.IsNullOrWhiteSpace(outlookAttachment.ContentId))
                            attachment.Filename = outlookAttachment.ContentId;
                        attachment.Data = outlookAttachment.Data;



                        emailInfo.Attachments.Add(attachment);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }

                }
            }
        }

        private void AddRecipients(List<string> whos, DWOutlookStorage.Message outlookMessage, RecipientType recipientType)
        {
            foreach (var recipient in outlookMessage.Recipients.Where(r => r.Type == recipientType))
            {
                if (recipient.Email != null && !whos.Contains(recipient.Email))
                {
                    whos.Add(recipient.Email);
                }
                if (recipient.DisplayName != null && !whos.Contains(recipient.DisplayName))
                {
                    whos.Add(recipient.DisplayName);
                }


            }
        }
    }
}

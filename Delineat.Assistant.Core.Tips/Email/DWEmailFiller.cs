using Delineat.Assistant.Core.Tips.Extensions;
using Delineat.Assistant.Core.Tips.Helpers;
using Delineat.Assistant.Core.Tips.Interfaces;
using Delineat.Assistant.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Delineat.Assistant.Core.Tips.Email
{
    public class DWEmailFiller : IDWTipsFiller
    {
        private readonly IDWEmailReader emailReader;

        public DWEmailFiller(IDWEmailReader emailReader)
        {
            this.emailReader = emailReader;
        }

        public DWTips Fill(object obj, IDWTipsAttachmentsStore attachmentsStore = null)
        {
            DWTips tips = new DWTips();

            var message = emailReader.GetEmailInfo(obj);
            if (message != null)
            {
                string from = message.From;
                List<string> to = new List<string>();
                List<string> cc = new List<string>();


                if (message.DeliverDateUtc.HasValue)
                    tips.Dates.Add(message.DeliverDateUtc.Value.ToLocalTime());
                tips.Descriptions.Add(message.Subject);
                //Rimosso tag di default email
                //tips.Tags.Add("EMAIL");

                AddEmails(to, message.To);
                AddEmails(cc, message.CC);
                AddEmails(cc, message.BCC);

                if (from != null && DWEmailHelper.IsInternalEmailAddress(message.From))
                {
                    bool allInternal = CheckAllInternal(message.To);
                    if (allInternal) allInternal = CheckAllInternal(message.CC);
                    if (allInternal) allInternal = CheckAllInternal(message.BCC);

                    if (allInternal)
                        tips.ItemTypes.Add(ItemType.Internal);
                    else
                        tips.ItemTypes.Add(ItemType.Outcoming);
                }
                else
                {
                    tips.ItemTypes.Add(ItemType.Incoming);
                }

                if (tips.ItemTypes.Contains(ItemType.Outcoming))
                {
                    tips.Whos.AddRange(to);
                    tips.Whos.AddRange(cc);
                    tips.Whos.Add(from);
                }
                else
                {
                    var domainName = DWEmailHelper.GetDomainName(from);
                    if (!string.IsNullOrWhiteSpace(domainName)) tips.Whos.Add(domainName);
                    tips.Whos.Add(from);
                    tips.Whos.AddRange(to);
                    tips.Whos.AddRange(cc);
                }


                if (message.Attachments != null)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        if (IsValidAttachment(attachment))
                        {
                            string filename = !string.IsNullOrEmpty(attachment.Filename) ? attachment.Filename : $"Allegato {message.Attachments.IndexOf(attachment)}";
                            tips.Attachments.Add(filename);
                            if (attachmentsStore != null)
                            {
                                attachmentsStore.StoreFile(attachment.Filename, attachment.Data);
                            }
                        }
                    }
                }
            }
            return tips;
        }

        private bool IsValidAttachment(DWEmailAttachment attachment)
        {
            switch (Path.GetExtension((attachment.Filename ?? string.Empty).ToLower()))
            {
                case ".gif":
                case ".png":
                case ".jpg":
                    return attachment.Data != null && attachment.Data.LongLength >= 8192;
                default:
                    return true;
            }

        }

        private void AddEmails(List<string> emailsTarget, IEnumerable<string> emails)
        {
            foreach (var email in emails)
            {
                var domainName = DWEmailHelper.GetDomainName(email);
                if (!string.IsNullOrEmpty(domainName) && !emailsTarget.Contains(domainName))
                    emailsTarget.Add(domainName);
                var domain = DWEmailHelper.GetDomain(email);
                if (!string.IsNullOrWhiteSpace(domain) && !emailsTarget.Contains(domain))
                    emailsTarget.Add(domain);
            }
        }

        private bool CheckAllInternal(IEnumerable<string> emails)
        {
            bool found = emails.Count() == 0;
            foreach (var email in emails)
            {
                if (email.IsEmail())
                {
                    if (!DWEmailHelper.IsInternalEmailAddress(email))
                    {
                        return false;
                    }
                    found = true;
                }
            }
            return found;
        }
    }
}

using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Exceptions;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Delineat.Assistant.API.WorkerJobs
{
    public class DANoteNotifierJob : IDAWorkerJob
    {
        private readonly DAEmailConfiguration emailConfiguration;
        private readonly ILogger logger;
        private readonly DAStoresConfiguration storesConfiguration;
        private readonly IDAStore store;

        public DANoteNotifierJob(IOptions<DAStoresConfiguration> storesConfiguration, IOptions<DAEmailConfiguration> emailConfiguration, ILoggerFactory loggerFactory, IDAStore store)
        {
            this.storesConfiguration = storesConfiguration.Value;
            this.emailConfiguration = emailConfiguration.Value;
            if (loggerFactory != null)
                this.logger = loggerFactory.CreateLogger<DANoteNotifierJob>();
            this.store = store;
        }


        public bool Execute()
        {
            try
            {

                foreach (var note in store.GetUnremindedNotes())
                {
                    if (note.RemainderDate <= DateTime.Now)
                    {
                        string subject = "Promemoria";
                        var noteParent = store.GetNoteScope(note);

                        if (noteParent.Job != null)
                        {
                            subject += $" - {noteParent.Job.Code} - {noteParent.Job.Description}";
                        }

                        if (noteParent.Item != null)
                        {
                            subject += $" - {noteParent.Item.Description}";
                        }

                        if (SendRemainderEmail(subject, note))
                        {
                            note.RemaindedDate = DateTime.Now;
                            store.UpdateNote(note);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (logger != null) logger.LogCritical(ex, "Impossibile notificare email");
            }
            return true;
        }

        private bool SendRemainderEmail(string subject, DWNote note)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Port = emailConfiguration.Port;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = emailConfiguration.SmtpServer;
                client.Credentials = new NetworkCredential(emailConfiguration.EmailUsername, emailConfiguration.EmailPassword);
                client.EnableSsl = emailConfiguration.EnableSsl;
                mail.From = new MailAddress(emailConfiguration.SenderEmail);
                foreach (var email in note.Emails)
                {
                    mail.To.Add(new MailAddress(email));
                }

                mail.Subject = subject;
                mail.Body = note.Note;
                if (mail.To.Count > 0)
                {
                    client.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (logger != null) logger.LogCritical(ex, $"Impossibile inviare email {subject}");
                return false;
            }
        }
    }
}

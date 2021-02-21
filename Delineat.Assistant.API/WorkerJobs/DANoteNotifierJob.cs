using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Exceptions;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Models;
using Microsoft.Extensions.Logging;
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

        public DANoteNotifierJob(DAStoresConfiguration storesConfiguration, DAEmailConfiguration emailConfiguration, ILoggerFactory loggerFactory)
        {
            this.storesConfiguration = storesConfiguration;
            this.emailConfiguration = emailConfiguration;
            if (loggerFactory != null)
                this.logger = loggerFactory.CreateLogger<DANoteNotifierJob>();
        }

        protected List<IDAStore> GetStores()
        {
            var storeFactory = new Core.Stores.Factories.DAConfigurationStoresFactory(this.storesConfiguration,logger);
            var stores = storeFactory.CreateStores();
            if (stores == null || stores.Count == 0)
            {
                throw new DAApplicationException("Nessuno store configurato nel servizio");
            }
            return stores;
        }

        public bool Execute()
        {
            try
            {
                foreach (var store in GetStores())
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

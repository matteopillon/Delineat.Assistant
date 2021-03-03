using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Delineat.Assistant.Models;
using System;
using System.Linq;

namespace Delineat.Assistant.Core.ObjectFactories
{
    public class DADataObjectFactory
    {
        private readonly DAAssistantDBContext dbContext;

        public DADataObjectFactory(DAAssistantDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public User GetDBUser(DWUser user)
        {
            return new User()
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Nickname = user.Nickname,


            };
        }

        public Note GetDBNote(DWNote note)
        {
            Note dbNote = new Note()
            {
                InsertDate = DateTime.Now,
                Text = note.Note,
                NoteType = ((char)note.NoteType).ToString(),
                ReminderDate = note.IsRemainder ? note.RemainderDate : null,
                RemindedDate = null,
                ReminderType = (int)(note.IsRemainder ? NoteReminderType.Email : NoteReminderType.None),

            };
            foreach (var recipient in note.Emails)
                dbNote.NotesReminderRecipients.Add(new NotesReminderRecipient() { Email = recipient, SentDate = null });

            return dbNote;
        }

        public Topic GetDBTopic(DWTopic topic)
        {
            Topic dbTopic = new Topic()
            {
                InsertDate = DateTime.Now,
                Description = topic.Description,
                Color = topic.Color
            };
            return dbTopic;
        }

        public Job GetDBJob(DWJob job)
        {
            Job dbJob = new Job()
            {
                JobId =job.JobId,
                InsertDate = DateTime.Now,
                Description = job.Description,
                Code = job.Code,
                Customer = job.Customer != null ? dbContext.Customers.FirstOrDefault(c=>c.CustomerId == job.Customer.CustomerId): null,
                CustomerInfo = job.CustomerInfo,
                OrderRef = job.OrderRef,
                QuotationRef = job.QuotationRef,

            };

            return dbJob;
        }


        private Customer GetDBCustomer(DWCustomer customer)
        {
            if (customer != null)
            {
                return new Customer()
                {
                    Code = customer.Code,
                    CustomerId = customer.CustomerId,
                    Description = customer.Description,
                    Domain = customer.Domain
                };
            }
            else
            {
                return null;
            }
        }

        public JobCode GetDBJobCode(DWJobCode jobCode)
        {
            JobCode dbJob = new JobCode()
            {

                Code = jobCode.Code.Trim().ToUpper(),
                CodeId = jobCode.CodeId,
                Note = jobCode.Note
            };

            return dbJob;
        }
        public WorkLog GetDBWorkLog(DWWorkLog workLog)
        {
            return new WorkLog()
            {
                AssignedTo = (int)workLog.AssignedTo,
                EndDate = workLog.EndDate,
                ExtimatedBeginDate = workLog.ExtimatedBeginDate,
                ExtimatedEndDate = workLog.ExtimatedEndDate,
                ExtimatedHour = (int)workLog.ExtimatedHour.TotalMinutes,
                StartDate = workLog.StartDate,
                Note = workLog.Note,
                Status = (int)workLog.Status,
                WorkedHour = (int)workLog.WorkedHour.TotalMinutes,
                WorkLogId = workLog.WorkLogId,
                WorkType = null
            };
        }

        public DayWorkType GetDBDayWorkType(DWDayWorkType dayWorkType)
        {
            if (dayWorkType != null)
            {
                return new DayWorkType()
                {
                    Code = dayWorkType.Code,
                    DayWorkTypeId = dayWorkType.DayWorkTypeId,
                    Description = dayWorkType.Description

                };
            }
            else
            {
                return null;
            }
        }

        public DayWorkLog GetDBDayWorkLog(DWDayWorkLog dayWorkLog)
        {
            if (dayWorkLog != null)
            {
                return new DayWorkLog()
                {
                    Date = dayWorkLog.Date,
                    DayWorkLogId = dayWorkLog.DayWorkLogId,
                    Job = GetDBJob(dayWorkLog.Job),
                    Minutes = dayWorkLog.Minutes,
                    User = GetDBUser(dayWorkLog.User),
                    WorkType = GetDBDayWorkType(dayWorkLog.WorkType)

                };
            }
            else
            {
                return null;
            }
        }
    }
}

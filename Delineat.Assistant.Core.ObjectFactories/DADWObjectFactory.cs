using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Delineat.Assistant.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.ObjectFactories
{
    public class DADWObjectFactory
    {
        private readonly DAAssistantDBContext dbContext;

        public DADWObjectFactory(DAAssistantDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DWUser GetDWUser(User user)
        {

            return user == null ? null : new DWUser()
            {
                UserId = user.UserId,
                Email = user.Email,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Nickname = user.Nickname,

            };
        }

        public DWJob GetDWJob(Job job)
        {
            if (job != null)
            {
                DWJob dwJob = new DWJob()
                {
                    JobId = job.JobId,
                    Code = job.Code,
                    Description = job.Description,
                    Path = job.Path,
                    CustomerInfo = job.CustomerInfo,
                    OrderRef = job.OrderRef,
                    QuotationRef = job.QuotationRef,
                    Group = GetDWJobGroup(job.Group)
                };
                if (job.Customer != null)
                {
                    dwJob.Customer = GetDWCustomer(job.Customer);
                }


                if (job.Topics != null)
                {
                    foreach (var topic in job.Topics)
                    {
                        dwJob.Topics.Add(GetDWTopic(topic));
                    }
                }

                if (job.Tags != null)
                {
                    foreach (var tag in job.Tags)
                    {
                        dwJob.Tags.Add(GetDWTag(tag.Tag));
                    }
                }

                if (job.Codes != null)
                {
                    foreach (var code in job.Codes)
                    {
                        dwJob.Codes.Add(GetDWJobCode(code));
                    }
                }

                if (job.SubJobs != null)
                {
                    foreach (var subJob in job.SubJobs)
                    {
                        dwJob.SubJobs.Add(GetDWSubJob(subJob));
                    }
                }

                return dwJob;
            }
            else
            {
                return null;
            }
        }

        public DWDayWorkType GetDWDayWorkType(DayWorkType dayWorkType)
        {
            if (dayWorkType != null)
            {
                return new DWDayWorkType()
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

        public DWDayWorkLog GetDWDayWorkLog(DayWorkLog dayWorkLog)
        {
            if (dayWorkLog != null)
            {
                return new DWDayWorkLog()
                {
                    Date = dayWorkLog.Date,
                    DayWorkLogId = dayWorkLog.DayWorkLogId,
                    Job = GetDWJob(dayWorkLog.Job),
                    Minutes = dayWorkLog.Minutes,
                    User = GetDWUser(dayWorkLog.User),
                    WorkType = GetDWDayWorkType(dayWorkLog.WorkType),
                    SubJob = GetDWSubJob(dayWorkLog.SubJob)
                };
            }
            else
            {
                return null;
            }
        }

        private DWSubJob GetDWSubJob(SubJob subJob)
        {
            if (subJob != null)
            {
                var result = new DWSubJob()
                {
                    Description = subJob.Description,
                    //Job = GetDWJob(subJob.Job),
                    Parent = GetDWSubJob(subJob.Parent),
                    SubJobId = subJob.SubJobId,
                };

                result.SubJobs = subJob.SubJobs?.Select(sj => GetDWSubJob(sj)).ToArray() ?? new DWSubJob[0];

                return result;
            }
            else
            {
                return null;
            }
        }

        private DWCustomer GetDWCustomer(Customer customer)
        {
            if (customer != null)
            {
                return new DWCustomer()
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

        private DWGroup GetDWJobGroup(JobGroup group)
        {
            if (group != null)
            {
                return new DWGroup() { GroupId = group.GroupId, Name = group.Name, IsCurrent = group.IsCurrent };
            }
            else
            {
                return null;
            }
        }

        private DWJobCode GetDWJobCode(JobCode code)
        {
            return new DWJobCode() { Code = code.Code, CodeId = code.CodeId, Note = code.Note };
        }

        private DWTopic GetDWTopic(Topic topic)
        {
            return new DWTopic() { TopicId = topic.TopicId, Color = topic.Color, Description = topic.Description, JobId = topic.Job.JobId };
        }

        public DWItem GetDWItem(Item item)
        {

            DWItem dwItem = new DWItem();
            dwItem.ItemId = item.ItemId;
            dwItem.Date = item.ReferenceDate;
            dwItem.Description = item.Description;
            if (item.ItemSource.Length > 0)
                dwItem.ItemType = (ItemType)item.ItemSource[0];
            dwItem.JobId = item.JobId;
            dwItem.Who = item.Who;
            dwItem.Path = item.Path;
            if (item.Notes != null)
            {
                foreach (var note in item.Notes)
                {
                    if (!note.Note.DeleteDate.HasValue)
                        dwItem.Notes.Add(GetDWNote(note.Note));
                }
            }
            if (item.Tags != null)
            {
                foreach (var tag in item.Tags)
                {
                    if (!tag.Tag.DeleteDate.HasValue)
                        dwItem.Tags.Add(GetDWTag(tag.Tag));
                }
            }

            if (item.Documents != null)
            {
                foreach (var doc in item.Documents)
                {
                    if (!doc.DeleteDate.HasValue)
                        dwItem.Documents.Add(GetDWDocument(doc));
                }
            }

            if (item.Topics != null)
            {
                foreach (var t in item.Topics)
                {
                    if (!t.Topic.DeleteDate.HasValue)
                        dwItem.Topics.Add(GetDWTopic(t.Topic));
                }
            }

            if (item.WorkLogs != null)
            {
                foreach (var w in item.WorkLogs)
                {
                    if (!w.WorkLog.DeleteDate.HasValue)
                        dwItem.WorkLogs.Add(GetDWWorkLog(w.WorkLog));
                }
            }

            return dwItem;
        }

        public NoteType GetNoteType(string noteType)
        {
            if (!string.IsNullOrWhiteSpace(noteType) && noteType.Length == 1)
            {
                try
                {
                    return (NoteType)noteType[0];
                }
                catch
                {
                    return NoteType.NotSet;
                }
            }
            else
            {
                return NoteType.NotSet;
            }
        }

        public DWDocument GetDWDocument(Document document)
        {
            DWDocument doc = new DWDocument();
            doc.DocumentId = document.DocumentId;
            doc.OpenedCount = document.OpenedCount;

            if (document.Versions != null)
            {
                foreach (var version in document.Versions)
                {
                    doc.Versions.Add(GetDWDocumentVersion(version));
                }
            }
            if (document.Tags != null)
            {
                foreach (var tag in document.Tags)
                {
                    doc.Tags.Add(GetDWTag(tag.Tag));
                }
            }

            if (document.Notes != null)
            {
                foreach (var note in document.Notes)
                {
                    doc.Notes.Add(GetDWNote(note.Note));
                }
            }

            return doc;
        }

        public string GetDocumentVersionPath(DocumentVersion version)
        {
            if (version != null)
                return Path.Combine(version.Document.Item.Job.Group.Path, version.Document.Item.Job.Path, version.Document.Item.Path, $"{version.Filename}{version.Extension}");
            else
                return string.Empty;
        }

        public DWDocumentVersion GetDWDocumentVersion(DocumentVersion version)
        {
            DWDocumentVersion ver = new DWDocumentVersion();
            ver.DocumentVersionId = version.DocumentVersionId;
            ver.RelativePath = version.RelativePath ?? string.Empty;
            ver.Filename = version.Filename;
            try
            {
                ver.Status = (DocumentVersionStatus)version.Status;
            }
            catch
            {
                ver.Status = DocumentVersionStatus.Valid;
            }
            ver.StatusSince = version.StatusSince;
            ver.InEvidence = version.InEvidence;
            ver.Reply = version.Reply;
            ver.WaitingForReply = version.WaitingForReply;
            ver.Extension = version.Extension;
            ver.InsertDate = version.InsertDate;
            if (version.Thumbnails != null)
            {
                foreach (var thumb in version.Thumbnails)
                {
                    ver.Thumbnails.Add(GetDWThumbnail(thumb));
                }
            }

            ver.PhysicalPath = GetDocumentVersionPath(version);

            return ver;
        }

        public DWThumbnail GetDWThumbnail(Thumbnail thumb)
        {
            DWThumbnail thumbnail = new DWThumbnail();
            thumbnail.ThumbnailId = thumb.ThumbnailId;
            thumbnail.Title = thumb.Title;
            return thumbnail;
        }

        public DWTag GetDWTag(Tag tag)
        {
            if (tag != null)
            {
                DWTag dwTag = new DWTag()
                {
                    Description = tag.Description,
                    TagId = tag.TagId,
                };

                return dwTag;
            }
            return null;
        }

        public DWNote GetDWNote(Note note)
        {
            if (note != null)
            {
                DWNote dwNote = new DWNote()
                {
                    Emails = note.NotesReminderRecipients.Select(r => r.Email).ToArray(),
                    Id = note.NoteId,
                    InsertDate = note.InsertDate,
                    IsRemainder = note.ReminderType != (int)NoteReminderType.None,
                    NoteType = GetNoteType(note.NoteType),
                    Note = note.Text,
                    RemainderDate = note.ReminderDate
                };

                if (note.Topics != null)
                {
                    foreach (var t in note.Topics)
                    {
                        dwNote.Topics.Add(GetDWTopic(t.Topic));
                    }
                }

                if (note.Tags != null)
                {
                    foreach (var t in note.Tags)
                    {
                        dwNote.Tags.Add(GetDWTag(t.Tag));
                    }
                }

                return dwNote;
            }
            else
            {
                return null;
            }
        }

        public Topic GetDWTopic(DWTopic topic)
        {
            throw new NotImplementedException();
        }

        public DWWorkLogType GetDWWorkLogType(WorkLogType workLogType)
        {
            if (workLogType != null)
            {
                return new DWWorkLogType()
                {
                    Description = workLogType.Description,
                    Order = workLogType.Order,
                    WorkLogTypeId = workLogType.WorkLogTypeId
                };
            }
            else
            {
                return null;
            }
        }

        public DWWorkLog GetDWWorkLog(WorkLog workLog)
        {
            WorkLogAssignedType assignment = WorkLogAssignedType.None;
            try
            {
                assignment = (WorkLogAssignedType)workLog.AssignedTo;
            }
            catch
            {
                assignment = WorkLogAssignedType.None;
            }

            WorkLogStatus status = WorkLogStatus.None;
            try
            {
                status = (WorkLogStatus)workLog.Status;
            }
            catch
            {
                status = WorkLogStatus.None;
            }

            return new DWWorkLog()
            {
                AssignedTo = assignment,
                EndDate = workLog.EndDate,
                ExtimatedBeginDate = workLog.ExtimatedBeginDate,
                ExtimatedEndDate = workLog.ExtimatedEndDate,
                ExtimatedHour = new TimeSpan(0, workLog.ExtimatedHour, 0),
                StartDate = workLog.StartDate,
                Note = workLog.Note,
                Status = status,
                WorkedHour = new TimeSpan(0, workLog.WorkedHour, 0),
                WorkLogId = workLog.WorkLogId,
                WorkType = this.GetDWWorkLogType(workLog.WorkType)
            };


        }
    }
}

using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.ObjectFactories;
using Delineat.Assistant.Core.Stores.Factories;
using Delineat.Assistant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Delineat.Assistant.Core.Stores
{
    public class DADBFolderStore : IDAStore
    {
        private readonly ILogger<DADBFolderStore> logger;
        private DAAssistantDBContext dataContext;
        private DADWObjectFactory dwObjectFactory;
        private DADataObjectFactory dataObjectFactory;

        private string folderStoreName = string.Empty;
        private string folderStoreDescription = string.Empty;

        public string Name { get => folderStoreName; set => folderStoreName = value; }
        public string Description { get => folderStoreDescription; set => folderStoreDescription = value; }

        public Dictionary<string, string> settings = new Dictionary<string, string>();


        public DADBFolderStore(ILogger<DADBFolderStore> logger,DAAssistantDBContext dataContext)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.dataObjectFactory = new DADataObjectFactory();
            this.dwObjectFactory = new DADWObjectFactory(dataContext);
        }


        public List<IDASyncStore> SyncStores { get; set; }

        public bool InitStore()
        {
            dataContext.Database.Migrate();
            return true;

        }

        public bool AddJob(DWJob job)
        {
            var foundJob = GetExistingJob(job);
            if (foundJob == null)
            {
                dataContext.Jobs.Add(dataObjectFactory.GetDBJob(job));
            }
            else
            {
                throw new Exceptions.DAStoresException($"Commessa con codice {job.Code} già presente");
            }
            return true;

        }

        private IQueryable<Job> GetJobsQuery()
        {
            return dataContext.Jobs
                .Include(j => j.Group)
                .Include(j => j.Topics)
                .Include(j => j.Customer)
                .Include(j => j.Codes)
                .Include(j => j.SubJobs)
                .Include(j => j.Tags).ThenInclude(t => t.Tag).Where(j => !j.DeleteDate.HasValue);
        }

        private Job GetJobFromId(int jobId)
        {
            var job = (from j in GetJobsQuery() where j.JobId == jobId select j).FirstOrDefault();
            return job;
        }

        private Item GetItemFromId(int itemId)
        {
            var dbItem = dataContext.Items.Include(i => i.Tags).ThenInclude(dt => dt.Tag)
                            .Include(d => d.Topics).ThenInclude(it => it.Topic)
                            .Include(d => d.WorkLogs).ThenInclude(wl => wl.WorkLog)
                            .Include(d => d.Job).ThenInclude(i => i.Group).FirstOrDefault(d => d.ItemId == itemId);
            return dbItem;
        }

        private Item GetItemDetailFromId(int itemId)
        {
            return dataContext.Items
                  .Include(i => i.Notes).ThenInclude(n => n.Note).ThenInclude(n => n.NotesReminderRecipients)
                  .Include(i => i.Notes).ThenInclude(n => n.Note).ThenInclude(n => n.Topics).ThenInclude(t => t.Topic)
                  .Include(i => i.Documents).ThenInclude(d => d.Versions).ThenInclude(v => v.Thumbnails)
                  .Include(i => i.Documents).ThenInclude(t => t.Tags).ThenInclude(v => v.Tag)
                  .Include(i => i.Tags).ThenInclude(t => t.Tag)
                  .Include(i => i.Topics).ThenInclude(t => t.Topic)
                  .Where(i => i.ItemId == itemId).FirstOrDefault();
        }

        private Document GetDocumentFromId(int documentId)
        {
            var dbDocument = dataContext.Documents.Include(d => d.Tags).ThenInclude(dt => dt.Tag)
                            .Include(d => d.Versions)
                            .Include(d => d.Item).ThenInclude(i => i.Job).ThenInclude(j => j.Group).FirstOrDefault(d => d.DocumentId == documentId);
            return dbDocument;
        }

        private WorkLog GetWorkLogFromId(int workLogId)
        {
            var dbWorkLog = dataContext.WorkLogs.FirstOrDefault(d => d.WorkLogId == workLogId);
            return dbWorkLog;
        }

        public DWJob GetJob(int jobId)
        {
            try
            {
                var job = GetJobFromId(jobId);
                var dwJob = dwObjectFactory.GetDWJob(job);

                return dwJob;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Errore nel recupero del job {jobId}");
                throw;
            }
        }

        public DWItem GetItem(int itemId)
        {
            try
            {
                var item = GetItemDetailFromId(itemId);
                var dwItem = dwObjectFactory.GetDWItem(item);

                return dwItem;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossibile recuperare l'item {itemId}");
                throw;

            }
        }

        public List<string> GetJobCategories(int jobId)
        {
            var job = GetJobFromId(jobId);
            List<string> categories = new List<string>();
            return categories;
        }

        public List<DWItem> GetJobItems(int jobId)
        {
            try
            {
                List<DWItem> items = new List<DWItem>();
                foreach (var item in dataContext.Items
                    .Include(i => i.Notes).ThenInclude(n => n.Note).ThenInclude(n => n.NotesReminderRecipients)
                    .Include(i => i.Notes).ThenInclude(n => n.Note).ThenInclude(n => n.Topics).ThenInclude(t => t.Topic)
                    .Include(i => i.Documents).ThenInclude(d => d.Versions).ThenInclude(v => v.Thumbnails)
                    .Include(i => i.Documents).ThenInclude(t => t.Tags).ThenInclude(v => v.Tag)
                    .Include(i => i.Tags).ThenInclude(t => t.Tag)
                    .Include(i => i.Topics).ThenInclude(t => t.Topic)
                    .Include(i => i.WorkLogs).ThenInclude(w => w.WorkLog)
                    .Where(i => i.JobId == jobId && !i.DeleteDate.HasValue).Select(i => dwObjectFactory.GetDWItem(i)))
                {
                    items.Add(item);
                }
                return items;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossibile recuperare gli item del job {jobId}");
                throw ;
            }
        }

        public List<DWNote> GetJobNotes(int jobId)
        {
            try
            {
                List<DWNote> notes = new List<DWNote>();
                foreach (var note in dataContext.Notes
                    .Include(n => n.NotesReminderRecipients)
                    .Include(n => n.Topics).ThenInclude(t => t.Topic)
                    .Include(n => n.Jobs)
                    .Where(n => n.Jobs.Count() > 0 && n.Jobs.FirstOrDefault().JobId == jobId).Select(n => dwObjectFactory.GetDWNote(n)))
                {
                    notes.Add(note);
                }
                return notes;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Errore durante il recupero delle Note del job {jobId}");
                throw;
            }
        }

        public List<DWJob> GetJobs()
        {
            List<Job> jobs = GetJobsQuery().ToList();

            bool saveChanges = false;
            foreach (var job in jobs.Where(j => j.Path == null))
            {
                DADelineatJobsFactory jobFactory = new DADelineatJobsFactory(logger); ;
                job.Path = Trim(jobFactory.CreateFolderNameFromJob(dwObjectFactory.GetDWJob(job)));
                saveChanges = true;
            }
            if (saveChanges) dataContext.SaveChanges();

            return jobs.Select(j => dwObjectFactory.GetDWJob(j)).ToList();
        }

        public void LoadSettings(Dictionary<string, string> settings)
        {
            this.settings = settings;
        }

        public DWStoreInfo AddJobTag(int jobId, DWTag tag)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var job = GetJobFromId(jobId);
                if (job != null)
                {
                    //verifico se esiste già un tag con la descrizione
                    Tag dbTag = GetOrCreateTag(tag.Description);

                    JobsTags jobTags = new JobsTags();
                    jobTags.Tag = dbTag;
                    jobTags.Job = job;

                    job.Tags.Add(jobTags);

                    dataContext.SaveChanges(true);
                    tag.TagId = dbTag.TagId;
                }
                else
                {
                    result.ErrorMessages.Add($"Commessa '{jobId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        private Tag GetExistingTag(string description)
        {
            var tag = dataContext.Tags.Where(t => t.Description == description).FirstOrDefault();
            return tag;
        }

        public Tag GetOrCreateTag(string description)
        {
            Tag dbTag = GetExistingTag(description);
            if (dbTag == null)
            {
                dbTag = new Tag();
                dbTag.Description = description;
                dbTag.InsertDate = DateTime.Now;
                dataContext.Tags.Add(dbTag);
            }
            return dbTag;
        }

        public DWStoreInfo AddNoteToJob(int jobId, DWNote note)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var job = GetJobFromId(jobId);
                if (job != null)
                {
                    var dbNote = dataObjectFactory.GetDBNote(note);
                    job.Notes.Add(new JobsNotes()
                    {
                        Note = dbNote,
                        Job = job
                    });
                    dataContext.SaveChanges(true);
                }
                else
                {
                    result.ErrorMessages.Add($"Commessa '{jobId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }


        public DWStoreInfo AddNoteToItem(int itemId, DWNote note)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var item = GetItemFromId(itemId);
                if (item != null)
                {
                    var dbNote = dataObjectFactory.GetDBNote(note);
                    item.Notes.Add(new ItemsNotes()
                    {
                        Note = dbNote,
                        Item = item
                    });
                    dataContext.SaveChanges(true);
                }
                else
                {
                    result.ErrorMessages.Add($"Commessa '{itemId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        private int SaveChangesAndSync()
        {
            return dataContext.SaveChanges(true);
        }

        public DWStoreInfo Store(DWTopic topic)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                if (topic == null) throw new ArgumentNullException(nameof(topic));
                Job dbJob = GetJobFromId(topic.JobId);
                if (dbJob == null)
                    throw new ApplicationException($"Commessa con id {topic.JobId} non trovata");

                Topic dbTopic = null;
                if (topic.TopicId == 0)
                {
                    dbTopic = dataObjectFactory.GetDBTopic(topic);
                    dataContext.Add(dbTopic);
                }
                else
                {
                    dbTopic = GetTopicFromId(topic.TopicId);
                    if (dbTopic == null)
                        throw new ApplicationException($"Topic con id {topic.TopicId} non trovato per l'aggiornamento");
                }

                dbTopic.Description = topic.Description;
                dbTopic.Job = dbJob;
                dbTopic.Color = topic.Color;
                SaveChangesAndSync();
                //Salvo il nuovo id inserito
                topic.TopicId = dbTopic.TopicId;

            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        private Topic GetTopicFromId(int topicId)
        {
            throw new NotImplementedException();
        }




        public DWStoreInfo Store(DWJob job)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                if (job == null) throw new ArgumentNullException(nameof(job));
                Job dbJob = null;
                if (job.JobId == 0)
                {
                    var context = this.dataContext;
                    dbJob = dataObjectFactory.GetDBJob(job);

                    dbJob.Group = AssertCurrentGroup();
                    dataContext.Add(dbJob);
                }
                else
                {
                    dbJob = GetJobFromId(job.JobId);
                    if (dbJob == null)
                        throw new ApplicationException($"Commessa con id {job.JobId} non trovata per l'aggiornamento");
                }

                dbJob.Description = job.Description;
                dbJob.Code = job.Code;
                if (dbJob.Codes == null) dbJob.Codes = new List<JobCode>();


                List<JobCode> codesToAdd = new List<JobCode>();
                List<JobCode> codesToRemove = new List<JobCode>();

                if (job.Codes != null)
                {
                    foreach (var code in job.Codes)
                    {
                        if (!string.IsNullOrWhiteSpace(code.Code))
                        {
                            code.Code = code.Code.Trim().ToUpper();
                            if (code.CodeId == 0)
                            {
                                codesToAdd.Add(dataObjectFactory.GetDBJobCode(code));
                            }
                            else
                            {
                                var jobCode = dbJob.Codes.FirstOrDefault(jc => jc.CodeId == code.CodeId);
                                if (jobCode != null)
                                {
                                    jobCode.Code = code.Code;
                                    jobCode.Note = code.Note;
                                }
                            }
                        }
                    }
                }

                foreach (var code in dbJob.Codes)
                {
                    if (job.Codes.FirstOrDefault(d => d.CodeId == code.CodeId) == null)
                    {
                        codesToRemove.Add(code);
                    }
                }

                foreach (var rc in codesToRemove)
                {
                    dbJob.Codes.Remove(rc);
                }

                foreach (var ra in codesToAdd)
                {
                    dbJob.Codes.Add(ra);
                }



                var syncStore = GetSyncStore(dbJob.Group);
                syncStore?.SyncJob(job);
                dbJob.Path = Trim(job.Path);
                SaveChangesAndSync();
                //Salvo il nuovo id inserito
                job.JobId = dbJob.JobId;

            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        private IDASyncStore GetSyncStore(Job job)
        {
            if (job.Group == null) job = GetJobFromId(job.JobId);

            if (job.Group != null)
            {
                return GetSyncStore(job.Group);
            }
            else
            {
                return null;
            }

        }

        private IDASyncStore GetSyncStore(JobGroup group)
        {
            return SyncStores.FirstOrDefault(s => s.Name.ToLower() == group.Name.ToLower());
        }

        public bool SetDefaultSyncStore(IDASyncStore syncStore)
        {
            if (syncStore == null) return false;
            foreach (var group in dataContext.JobGroups)
            {
                group.IsCurrent = group.Name == syncStore.Name;
            }
            SaveChangesAndSync();
            return false;
        }

        private JobGroup AssertCurrentGroup()
        {
            var group = dataContext.JobGroups.Where(g => g.IsCurrent).SingleOrDefault();
            if (group == null)
            {
                throw new ApplicationException("Configurare un archivio predefinito");
            }
            return group;
        }

        public DWStoreInfoWithUpdatedData<int> Store(DWItem item, DWJob job)
        {
            DWStoreInfoWithUpdatedData<int> result = new DWStoreInfoWithUpdatedData<int>();
            try
            {

                var dbjob = GetJobFromId(item.JobId);
                if (dbjob != null)
                {
                    IDASyncStore syncStore = GetSyncStore(dbjob.Group);

                    var dbItem = new Item();
                    dbItem.InsertDate = DateTime.Now;
                    SetDWItem(item, dbItem, dbjob);

                    dataContext.Add(dbItem);

                    syncStore?.SyncItem(job, item);

                    dbItem.Path = Trim(item.Path);



                    dataContext.SaveChanges(true);
                    result.Data = dbItem.ItemId;
                }
                else
                {
                    result.ErrorMessages.Add($"Commessa '{item.JobId}' non trovata");
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        private void SetDWItem(DWItem item, Item dbItem, Job dbJob)
        {

            dbItem.ItemId = item.ItemId;
            dbItem.Description = item.Description ?? string.Empty;
            dbItem.Who = item.Who ?? string.Empty;
            dbItem.ItemSource = ((char)item.ItemType).ToString();
            dbItem.Job = dbJob;
            dbItem.ReferenceDate = item.Date;
            dbItem.Path = Trim(item.Path);
            foreach (var note in item.Notes)
            {
                ItemsNotes itemNotes = new ItemsNotes();
                itemNotes.Note = dataObjectFactory.GetDBNote(note);
                itemNotes.Item = dbItem;
                dbItem.Notes.Add(itemNotes);
            }

            SetVersions(dbItem, item.Attachments);


            foreach (var tag in item.Tags)
            {
                //Ricerco se il tag esiste già 
                var foundTag = dataContext.Tags.FirstOrDefault(t => t.Description.ToUpper() == tag.Description.ToUpper());
                if (foundTag == null)
                    foundTag = new Tag() { Description = tag.Description.ToUpper(), Color = string.Empty };

                dbItem.Tags.Add(new ItemsTags() { Tag = foundTag });
            }

            foreach (var topic in item.Topics)
            {
                //Ricerco se il tag esiste già 
                var foundTopic = dbJob.Topics.FirstOrDefault(t => t.Description.ToUpper() == topic.Description.ToUpper());
                if (foundTopic == null)
                {
                    foundTopic = dataObjectFactory.GetDBTopic(topic);
                    dbJob.Topics.Add(foundTopic);
                }
                dbItem.Topics.Add(new ItemsTopics() { Topic = foundTopic });
            }
        }

        private void SetVersions(Item dbItem, List<DWAttachment> attachments)
        {
            foreach (var attachment in attachments)
            {
                if (File.Exists(attachment.Path))
                {
                    string fileName = Path.GetFileNameWithoutExtension(attachment.Path);
                    string relativePath = string.Empty;
                    if (!string.IsNullOrEmpty(attachment.RootPath))
                    {
                        var indexOfItemPath = attachment.Path.IndexOf(attachment.RootPath) + attachment.RootPath.Length + 1;
                        fileName = Path.ChangeExtension(attachment.Path.Substring(indexOfItemPath), string.Empty);
                        if (fileName.Last() == '.') fileName = fileName.Substring(0, fileName.Length - 1);
                    }
                    //TODO: Copia file
                    DocumentVersion version = new DocumentVersion()
                    {
                        Extension = Path.GetExtension(attachment.Path),
                        RelativePath = Trim(relativePath),
                        Filename = Trim(fileName),
                        InsertDate = DateTime.Now
                    };
                    Document doc = new Document();

                    doc.Versions.Add(version);
                    doc.InsertDate = DateTime.Now;
                    dbItem.Documents.Add(doc);
                }
                else
                {

                }

            }

        }

        public bool UpdateItem(DWItem item, DWJob job)
        {
            return false;
        }

        public DWStoreInfo UpdateNote(DWNote note)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var dbNote = dataContext.Notes.Where(n => n.NoteId == note.Id).FirstOrDefault();
                if (dbNote != null)
                {
                    dbNote.Text = note.Note;
                    dbNote.RemindedDate = note.RemaindedDate;
                    dbNote.ReminderDate = note.RemainderDate;

                    dataContext.SaveChanges(true);
                }
                else
                {
                    result.ErrorMessages.Add($"Nota '{dbNote.NoteId}' non trovata");
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        public List<DWNote> GetUnremindedNotes()
        {
            return dataContext.Notes.Include(n => n.NotesReminderRecipients)
                .Where(n => !n.RemindedDate.HasValue && n.ReminderType != (int)NoteReminderType.None).Select(n => dwObjectFactory.GetDWNote(n)).ToList();
        }

        public DWScope GetNoteScope(DWNote note)
        {
            //Ricerco se è una nota di lavoro
            DWScope scope = new DWScope();

            if (note != null)
            {
                var dbNote = dataContext.Notes.Include(n => n.Customers).ThenInclude(c => c.Customer)
                    .Include(n => n.Documents)
                    .Include(n => n.Items)
                    .Include(n => n.Jobs)
                    .Include(n => n.NotesReminderRecipients)
                    .Where(n => n.NoteId == note.Id).FirstOrDefault();
                if (dbNote != null)
                {
                    switch (note.NoteType)
                    {
                        case NoteType.Customer:

                            break;
                        case NoteType.Item:
                            if (dbNote.Items != null)
                            {
                                scope.Item = dwObjectFactory.GetDWItem(dataContext.Items.FirstOrDefault(i => i.ItemId == dbNote.Items.FirstOrDefault().ItemId));
                                scope.Job = GetJob(scope.Item.JobId);
                            }
                            break;
                        case NoteType.Job:
                            if (dbNote.Jobs != null)
                            {
                                var dbJob = dbNote.Jobs.FirstOrDefault();
                                if (note != null)
                                    scope.Job = dwObjectFactory.GetDWJob(dbJob.Job);
                            }
                            break;
                    }
                }
            }
            return scope;
        }
        #region Sync

        public bool AlreadySync(IDASyncStore syncStore)
        {
            var findSync = dataContext.StoreSyncLogs.Include(s => s.Group).FirstOrDefault(s => s.Completed && s.Group.Name == syncStore.Name);
            return findSync != null;
        }

        StoreSyncLog syncLog = null;
        private ILogger syncLogger = null;

        public bool BeginSync(IDASyncStore store, ILogger syncLogger)
        {
            this.syncLogger = syncLogger;
            dataContext.JobGroups.Load();
            int groupCount = dataContext.JobGroups.Count();

            var existingGroup = dataContext.JobGroups.FirstOrDefault(g => g.Name == store.Name);
            if (existingGroup == null)
            {
                existingGroup = new JobGroup() { Name = store.Name, IsCurrent = groupCount == 0 };
                var fileStore = store as DAFileFolderStore;
                if (fileStore != null)
                {
                    existingGroup.Path = Trim(fileStore.FolderPath);
                }
            }
            syncLog = new StoreSyncLog()
            {
                Completed = false,
                InsertDate = DateTime.Now,
                TargetName = this.Name,
                Group = existingGroup

            };
            dataContext.StoreSyncLogs.Add(syncLog);
            dataContext.SaveChanges();
            return true;
        }

        private Job GetExistingJob(DWJob job)
        {
            var foundJob = dataContext.Jobs.Local.SingleOrDefault(j => j.Code == job.Code);
            if (foundJob == null)
            {

                foundJob = dataContext.Jobs.SingleOrDefault(j => j.Code == job.Code);
            }
            return foundJob;
        }

        public bool SyncJob(DWJob job)
        {
            var dbJob = GetExistingJob(job);
            if (dbJob == null)
            {
                dbJob = new Job();
                dbJob.Code = job.Code;
                dbJob.InsertDate = DateTime.Now;
                dbJob.Group = syncLog.Group;
                dbJob.Path = job.Path;
                dataContext.Jobs.Add(dbJob);
            }
            dbJob.Description = job.Description;
            dbJob.ExportSyncId = syncLog.SyncId;

            AddSyncLogEntry($"Sync job {job} completato");
            dataContext.SaveChanges();
            return true;
        }

        public void AddSyncLogEntry(string message)
        {
            syncLogger.LogInformation(message);
            syncLog.Entries.Add(new StoreSyncLogEntry() { Message = message });
        }

        public bool SyncItem(DWJob job, DWItem item)
        {
            var dbJob = GetExistingJob(job);
            if (dbJob != null)
            {
                AddSyncLogEntry($"Sync item {item.Description} completato");
                var dbItem = GetExistingItem(item, dbJob.JobId);
                if (dbItem == null)
                {
                    dbItem = new Item();
                    dbItem.InsertDate = DateTime.Now;
                    dataContext.Items.Add(dbItem);
                }

                SetDWItem(item, dbItem, dbJob);

                dbItem.ImportSyncId = syncLog.SyncId;
                foreach (var document in dbItem.Documents)
                {
                    if (document.ImportSyncId == 0)
                        document.ImportSyncId = dbItem.ImportSyncId;
                }
                foreach (var note in dbItem.Notes)
                {
                    if (note.Note.ImportSyncId == 0)
                        note.Note.ImportSyncId = dbItem.ImportSyncId;
                }
                dataContext.SaveChanges();

                return true;
            }
            else
            {
                syncLog.Entries.Add(new StoreSyncLogEntry() { Message = $"Impossibile recuperare la commessa con codice {job.Code}" });
                return false;
            }
        }

        private Item GetExistingItem(DWItem item, int jobId)
        {

            var foundItem = dataContext.Items.Local.SingleOrDefault(i => i.ReferenceDate == item.Date && item.Description == i.Description && i.Who == item.Who && item.JobId == jobId);
            if (foundItem == null)
            {
                foundItem = dataContext.Items.SingleOrDefault(i => i.ReferenceDate == item.Date && item.Description == i.Description && i.Who == item.Who && item.JobId == jobId);
            }

            if (foundItem != null)
            {
                var testType = ItemType.None;
                try
                {
                    testType = (ItemType)foundItem.ItemSource[0];
                }
                catch
                {
                    testType = ItemType.None;
                }
                if (testType == item.ItemType)
                    return foundItem;
            }
            return null;
        }

        public bool CompleteSync(IDASyncStore store)
        {
            try
            {
                syncLog.Completed = true;
                dataContext.SaveChanges();
                syncLog = null;
                syncLogger = null;
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Sincronizzazione completata con errori");
                return false;
            }
        }

        public bool TryRollbackSync(IDASyncStore store)
        {
            syncLog = null;
            syncLogger = null;
            return true;
        }


        public byte[] GetDocumentVersionData(int id)
        {
            var version = dataContext.DocumentVersions.Include(v => v.Document)
                    .ThenInclude(d => d.Item).ThenInclude(i => i.Job).ThenInclude(j => j.Group).Where(dv => dv.DocumentVersionId == id).FirstOrDefault();

            //Get FilePath

            string filename = dwObjectFactory.GetDocumentVersionPath(version);
            version.Document.OpenedCount++;
            dataContext.SaveChanges();


            return File.ReadAllBytes(filename);

        }

        public string GetDocumentVersionPath(int id)
        {
            var version = dataContext.DocumentVersions.Include(v => v.Document)
                    .ThenInclude(d => d.Item).ThenInclude(i => i.Job).ThenInclude(j => j.Group).Where(dv => dv.DocumentVersionId == id).FirstOrDefault();

            //Get FilePath
            string filename = dwObjectFactory.GetDocumentVersionPath(version);
            //Aggiorno il numero di richieste per il file

            return filename;

        }

        public List<DWTag> GetTags()
        {
            return dataContext.Tags.Include(jt => jt.Jobs).Select(t => dwObjectFactory.GetDWTag(t)).ToList();
        }

        public DWStoreInfo DeleteJob(int jobId)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var dbJob = GetJobFromId(jobId);

                if (dbJob != null)
                {
                    dbJob.DeleteDate = DateTime.Now;

                    GetSyncStore(dbJob).SyncDeleteJob(dwObjectFactory.GetDWJob(dbJob));

                    dataContext.SaveChanges(true);
                }
                else
                {
                    result.ErrorMessages.Add($"Job '{jobId}' non trovato");
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;

        }

        public DWStoreInfo DeleteItem(int itemId)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var dbItem = GetItemFromId(itemId);

                if (dbItem != null)
                {
                    dbItem.DeleteDate = DateTime.Now;

                    GetSyncStore(dbItem.Job).SyncDeleteItem(dwObjectFactory.GetDWJob(dbItem.Job), dwObjectFactory.GetDWItem(dbItem));

                    dataContext.SaveChanges(true);
                }
                else
                {
                    result.ErrorMessages.Add($"ItemId '{itemId}' non trovato");
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;

        }

        public List<DWItem> GetJobItems(DWJob job)
        {
            return GetJobItems(job.JobId);
        }

        public bool SyncDeleteJob(DWJob job)
        {
            return DeleteJob(job.JobId).Stored;
        }

        public bool SyncDeleteItem(DWJob job, DWItem item)
        {
            return DeleteItem(item.ItemId).Stored;
        }

        public DWStoreInfo DeleteDocument(int documentId)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var dbDocument = GetDocumentFromId(documentId);
                if (dbDocument != null)
                {
                    dbDocument.DeleteDate = DateTime.Now;

                    GetSyncStore(dbDocument.Item.Job).SyncDeleteDocument(dwObjectFactory.GetDWJob(dbDocument.Item.Job), dwObjectFactory.GetDWItem(dbDocument.Item), dwObjectFactory.GetDWDocument(dbDocument));

                    dataContext.SaveChanges(true);
                }
                else
                {
                    result.ErrorMessages.Add($"ItemId '{documentId}' non trovato");
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;

        }

        public bool SyncDeleteDocument(DWJob job, DWItem item, DWDocument document)
        {
            return DeleteDocument(document.DocumentId).Stored;
        }

        public DWStoreInfo RemoveItemTag(int itemId, int tagId)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var itemTag = dataContext.ItemsTags.Include(it => it.Tag)
                .Include(it => it.Item).ThenInclude(i => i.Job).ThenInclude(j => j.Group)
                .FirstOrDefault(it => it.ItemId == itemId && it.TagId == tagId);

                if (itemTag != null)
                {
                    dataContext.ItemsTags.Remove(itemTag);

                    GetSyncStore(itemTag.Item.Job).SyncRemoveItemTag(dwObjectFactory.GetDWJob(itemTag.Item.Job), dwObjectFactory.GetDWItem(itemTag.Item), dwObjectFactory.GetDWTag(itemTag.Tag));

                    SaveChangesAndSync();
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);
            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        public DWStoreInfo RemoveDocumentTag(int documentId, int tagId)
        {
            DWStoreInfo result = new DWStoreInfo();
            try
            {
                var documentTag = dataContext.DocumentsTags.Include(dt => dt.Tag)
                .Include(d => d.Document.Versions)
                .Include(dt => dt.Document).ThenInclude(d => d.Item).ThenInclude(i => i.Job).ThenInclude(j => j.Group)
                .FirstOrDefault(dt => dt.DocumentId == documentId && dt.TagId == tagId);

                if (documentTag != null)
                {
                    dataContext.DocumentsTags.Remove(documentTag);

                    GetSyncStore(documentTag.Document.Item.Job.Group).SyncRemoveDocumemtTag(dwObjectFactory.GetDWJob(documentTag.Document.Item.Job), dwObjectFactory.GetDWItem(documentTag.Document.Item), dwObjectFactory.GetDWDocument(documentTag.Document), dwObjectFactory.GetDWTag(documentTag.Tag));

                    SaveChangesAndSync();
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add(ex.Message);
            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        public bool SyncRemoveDocumemtTag(DWJob dWJob, DWItem dWItem, DWDocument dWDocument, DWTag dWTag)
        {
            return RemoveDocumentTag(dWDocument.DocumentId, dWTag.TagId).Stored;
        }

        public bool SyncRemoveItemTag(DWJob dWJob, DWItem dWItem, DWTag dWTag)
        {
            return RemoveItemTag(dWItem.ItemId, dWTag.TagId).Stored;
        }

        public DWStoreInfoWithUpdatedData<DWTag[]> AddDocumentTags(int documentId, DWTag[] tags)
        {
            DWStoreInfoWithUpdatedData<DWTag[]> result = new DWStoreInfoWithUpdatedData<DWTag[]>();
            try
            {
                var dbDocument = GetDocumentFromId(documentId);

                if (dbDocument != null)
                {
                    foreach (var tag in tags)
                    {
                        //verifico se esiste già un tag con la descrizione
                        Tag dbTag = GetOrCreateTag(tag.Description);
                        if (dbTag != null)
                        {
                            var existsTag = dbDocument.Tags.FirstOrDefault(t => t.TagId == dbTag.TagId);
                            if (existsTag != null) continue;
                        }

                        DocumentsTags documentTags = new DocumentsTags();
                        documentTags.Tag = dbTag;
                        documentTags.Document = dbDocument;

                        GetSyncStore(dbDocument.Item.Job.Group).SyncAddDocumentTag(dwObjectFactory.GetDWJob(dbDocument.Item.Job), dwObjectFactory.GetDWItem(dbDocument.Item), dwObjectFactory.GetDWDocument(dbDocument), dwObjectFactory.GetDWTag(dbTag), dwObjectFactory.GetDocumentVersionPath(dbDocument.Versions.FirstOrDefault()));

                        dbDocument.Tags.Add(documentTags);
                    }
                    dataContext.SaveChanges(true);

                    //Ricarico i tag del documento per restituirli
                    dbDocument = GetDocumentFromId(documentId);
                    result.Stored = true;
                    result.Data = dbDocument.Tags.Select(dt => dwObjectFactory.GetDWTag(dt.Tag)).ToArray();
                }
                else
                {
                    result.ErrorMessages.Add($"Commessa '{documentId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        public DWStoreInfoWithUpdatedData<DWTag[]> AddItemTags(int itemId, DWTag[] tags)
        {
            DWStoreInfoWithUpdatedData<DWTag[]> result = new DWStoreInfoWithUpdatedData<DWTag[]>();
            try
            {
                var dbItem = GetItemFromId(itemId);

                if (dbItem != null)
                {
                    foreach (var tag in tags)
                    {
                        //verifico se esiste già un tag con la descrizione
                        Tag dbTag = GetOrCreateTag(tag.Description);
                        if (dbTag != null)
                        {
                            var existsTag = dbItem.Tags.FirstOrDefault(t => t.TagId == dbTag.TagId);
                            if (existsTag != null) continue;
                        }

                        ItemsTags itemsTags = new ItemsTags();
                        itemsTags.Tag = dbTag;
                        itemsTags.Item = dbItem;


                        GetSyncStore(dbItem.Job.Group).SyncAddItemTag(dwObjectFactory.GetDWJob(dbItem.Job), dwObjectFactory.GetDWItem(dbItem), dwObjectFactory.GetDWTag(dbTag));

                        dbItem.Tags.Add(itemsTags);
                    }
                    dataContext.SaveChanges(true);

                    //Ricarico i tag del documento per restituirli
                    dbItem = GetItemFromId(itemId);
                    result.Stored = true;
                    result.Data = dbItem.Tags.Select(dt => dwObjectFactory.GetDWTag(dt.Tag)).ToArray();
                }
                else
                {
                    result.ErrorMessages.Add($"Commessa '{itemId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        public bool SyncAddDocumentTag(DWJob dWJob, DWItem dWItem, DWDocument dWDocument, DWTag dWTag, string fileVersionPath)
        {
            throw new NotImplementedException();
        }

        public bool SyncAddItemTag(DWJob dWJob, DWItem dWItem, DWTag dWTag)
        {
            throw new NotImplementedException();
        }

        #region Work Logs
        public List<DWWorkLogType> GetWorkLogTypes()
        {
            try
            {

                var workTypes = dataContext.WorkLogTypes.OrderBy(t => t.Order).Select(t => dwObjectFactory.GetDWWorkLogType(t)).ToList();
                //Se vuoto popolo il database
                if (workTypes.Count == 0)
                {
                    dataContext.WorkLogTypes.Add(new WorkLogType() { Description = "GENERICO", Order = 10 });
                    dataContext.WorkLogTypes.Add(new WorkLogType() { Description = "MODELLAZIONE 3D", Order = 20 });
                    dataContext.WorkLogTypes.Add(new WorkLogType() { Description = "ANALISI", Order = 30 });
                    dataContext.WorkLogTypes.Add(new WorkLogType() { Description = "RIUNIONE", Order = 40 });

                    SaveChangesAndSync();
                    return GetWorkLogTypes();
                }
                return workTypes;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Errore durante il recupero dei logTypes");
                throw;
            }

        }

        public DWStoreInfoWithUpdatedData<DWWorkLog> UpdateWorkLog(DWWorkLog workLog)
        {
            DWStoreInfoWithUpdatedData<DWWorkLog> result = new DWStoreInfoWithUpdatedData<DWWorkLog>();
            try
            {
                var dbWorkLog = GetWorkLogFromId(workLog.WorkLogId);

                if (dbWorkLog != null)
                {

                    dataContext.SaveChanges(true);

                    result.Stored = true;
                    result.Data = dwObjectFactory.GetDWWorkLog(dbWorkLog);
                }
                else
                {
                    result.ErrorMessages.Add($"Worklog '{workLog.WorkLogId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        public DWStoreInfoWithUpdatedData<DWWorkLog> AddWorkLogToItem(int itemId, DWWorkLog log)
        {
            DWStoreInfoWithUpdatedData<DWWorkLog> result = new DWStoreInfoWithUpdatedData<DWWorkLog>();
            try
            {
                var dbItem = GetItemFromId(itemId);

                if (dbItem != null)
                {
                    WorkLog dbLog = null;
                    if (dbItem.WorkLogs.Count == 0)
                    {
                        dbLog = dataObjectFactory.GetDBWorkLog(log);
                        dbItem.WorkLogs.Add(new ItemsWorkLogs() { WorkLog = dbLog });
                    }
                    else
                    {
                        dbLog = dbItem.WorkLogs.FirstOrDefault().WorkLog;
                        dbLog.ExtimatedHour = (int)log.ExtimatedHour.TotalMinutes;
                        dbLog.WorkedHour = (int)log.WorkedHour.TotalMinutes;
                    }

                    dataContext.SaveChanges(true);

                    //Ricarico i tag del documento per restituirli
                    dbItem = GetItemFromId(itemId);
                    result.Stored = true;
                    result.Data = dwObjectFactory.GetDWWorkLog(dbLog);
                }
                else
                {
                    result.ErrorMessages.Add($"Commessa '{itemId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }


        private DocumentVersion GetDocumentVersionFromId(int documentVersionId)
        {
            return dataContext.DocumentVersions.Include(d => d.Thumbnails).Include(d => d.Document)
                .ThenInclude(d => d.Item)
                .ThenInclude(i => i.Job).ThenInclude(j => j.Group).FirstOrDefault(d => d.DocumentVersionId == documentVersionId);
        }

        public DWDocumentVersion GetDocumentVersion(int documentVersionId)
        {
            var dbVersion = GetDocumentVersionFromId(documentVersionId);
            if (dbVersion != null)
            {
                return dwObjectFactory.GetDWDocumentVersion(dbVersion);
            }
            else
            {
                return null;
            }
        }

        public DWStoreInfoWithUpdatedData<DWDocumentVersion> UpdateDocumentVersion(DWDocumentVersion documentVersion)
        {
            DWStoreInfoWithUpdatedData<DWDocumentVersion> result = new DWStoreInfoWithUpdatedData<DWDocumentVersion>();
            try
            {
                var dbDocVersion = GetDocumentVersionFromId(documentVersion.DocumentVersionId);

                if (dbDocVersion != null)
                {
                    dbDocVersion.Extension = documentVersion.Extension;
                    dbDocVersion.Filename = Trim(documentVersion.Filename);
                    dbDocVersion.Status = (int)documentVersion.Status;
                    dbDocVersion.StatusSince = documentVersion.StatusSince;
                    dbDocVersion.InEvidence = documentVersion.InEvidence;
                    dbDocVersion.RelativePath = Trim(documentVersion.RelativePath);
                    dbDocVersion.Reply = documentVersion.Reply;
                    dbDocVersion.UpdateDate = DateTime.Now;
                    dbDocVersion.WaitingForReply = documentVersion.WaitingForReply;

                    dataContext.SaveChanges(true);

                    //Ricarico i tag del documento per restituirli               
                    result.Stored = true;
                    result.Data = GetDocumentVersion(documentVersion.DocumentVersionId);
                }
                else
                {
                    result.ErrorMessages.Add($"Versione documento '{documentVersion.DocumentVersionId}' non trovata");
                }
            }
            catch (Exception ex)
            {

                result.ErrorMessages.Add(ex.Message);

            }
            result.Stored = result.ErrorMessages.Count == 0;
            return result;
        }

        #endregion

        #endregion

        #region Utils
        private string Trim(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        public void UpdateSyncSettings(List<IDASyncStore> sourceSyncStores)
        {
            if (sourceSyncStores == null) return;
            foreach (var syncStore in sourceSyncStores)
            {
                var group = dataContext.JobGroups.FirstOrDefault(g => g.Name == syncStore.Name);
                if (group != null)
                {
                    var fileSyncStore = syncStore as DAFileFolderStore;
                    if (fileSyncStore != null)
                    {
                        group.Path = fileSyncStore.FolderPath;
                    }

                }
            }

            SaveChangesAndSync();

        }
        #endregion
    }
}


using Delineat.Assistant.Core.Helpers;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Exceptions;
using Delineat.Assistant.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Stores
{
    public class DAFileFolderStore : IDASyncStore
    {
        private const string FOLDER_PATH_SETTINGS = "rootDirectoryPath";
        private const string UTILS_LINK_GENERATOR = "linkgeneratorPath";
        private const string USE_CACHE = "useCache";
        private const string ITEM_INFO_JSON_FILE = "item.json";
        public const string CATEGORY_INFO_JSON_FILE = "category.json";
        public const string JOB_NOTES_JSON_FILE = "notes.json";
        private string folderPath = string.Empty;
        private readonly ILogger logger;
        private List<Interfaces.IDADelineatJobsFactory> jobsFactories = new List<Interfaces.IDADelineatJobsFactory>();

        public Dictionary<int, DWJob> cachedJobs = new Dictionary<int, DWJob>();

        private string linkGeneratorPath;
        private bool useCache;

        #region Costruttori
        public DAFileFolderStore(ILogger logger)
        {
            this.logger = logger;
            jobsFactories = new List<Interfaces.IDADelineatJobsFactory>();
            jobsFactories.Add(new Factories.DADelineatJobsFactory(logger));
        }
        #endregion

        #region Properties
        public string Description
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string FolderPath { get => folderPath; }

        #endregion

        #region IDWStore

        public void LoadSettings(Dictionary<string, string> settings)
        {
            if (settings != null)
            {
                if (settings.ContainsKey(FOLDER_PATH_SETTINGS))
                {
                    this.folderPath = settings[FOLDER_PATH_SETTINGS];
                }

                if (settings.ContainsKey(UTILS_LINK_GENERATOR))
                {
                    this.linkGeneratorPath = settings[UTILS_LINK_GENERATOR];
                }
                bool settingValue = true;
                if (settings.ContainsKey(USE_CACHE))
                {


                    if (bool.TryParse(settings[USE_CACHE], out settingValue))
                    {
                        settingValue = true;
                    }

                }
                this.useCache = settingValue;
            }
        }

        public DWStoreInfo Store(DWItem item, DWJob job)
        {

            string jobFolderPath = string.Empty;
            string itemFolderPath = string.Empty;

            if (SaveItemJson(item, job, out jobFolderPath, out itemFolderPath, true))
            {
                //Salvo i file
                foreach (var attachement in item.Attachments)
                {
                    string moveFilePath = DAFileHelper.GetNotExistingFileName(Path.Combine(itemFolderPath, GetRelativePath(attachement)));
                    if (!Directory.Exists(Path.GetDirectoryName(moveFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(moveFilePath));
                    }
                    System.IO.File.Move(attachement.Path, moveFilePath);
                    attachement.RootPath = itemFolderPath;
                    attachement.Path = moveFilePath;
                }
                //Per ogni tag creo una sottocartella (Se non esiste) e creo il link
                foreach (var tag in item.Tags)
                {
                    var jobFolder = Path.Combine(this.FolderPath, jobFolderPath);

                    string tagPath = GetOrCreateTagPath(job, tag);
                    if (!string.IsNullOrWhiteSpace(tagPath))
                    {
                        string categoryInfoFilePath = Path.Combine(tagPath, CATEGORY_INFO_JSON_FILE);
                        if (!File.Exists(categoryInfoFilePath)) File.WriteAllText(categoryInfoFilePath, string.Empty);

                        CreateShortcutInPath(tagPath, itemFolderPath, item.Path);
                    }
                }
            }

            return new DWStoreInfo() { Stored = true };
        }

        private string GetRelativePath(DWAttachment attachment)
        {
            string sessionRoot = string.Empty;

            return attachment.Path.Replace($"{attachment.RootPath}\\", string.Empty);

        }

        private void CreateShortcutInPath(string categoryPath, string filePath, string description)
        {
            string linkPath = string.Format("{0}.lnk", Path.Combine(categoryPath, Path.GetFileName(filePath)));

            if (File.Exists(this.linkGeneratorPath))
            {
                if (!Directory.Exists(categoryPath)) Directory.CreateDirectory(categoryPath);



                var startInfo = new ProcessStartInfo();
                startInfo.Arguments = string.Format("-s \"{0}\" -l \"{1}\" -d \"{2}\"", filePath, linkPath, description);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = categoryPath;

                startInfo.FileName = this.linkGeneratorPath;
                Process.Start(startInfo);
            }
            else
            {
                var wsh = new IWshRuntimeLibrary.WshShell();
                var shortcut = wsh.CreateShortcut(linkPath) as IWshRuntimeLibrary.IWshShortcut;
                shortcut.Arguments = string.Empty;
                shortcut.TargetPath = filePath;
                // not sure about what this is for
                shortcut.WindowStyle = 1;
                shortcut.Description = description;
                shortcut.WorkingDirectory = Path.GetDirectoryName(filePath);
                //shortcut.IconLocation = string.Empty;
                shortcut.Save();
            }
        }

        public List<DWJob> GetJobs()
        {
            if (cachedJobs != null || cachedJobs.Count == 0)
            {
                cachedJobs = new Dictionary<int, DWJob>();
                int i = 1;
                AssertFolderExists();
                foreach (var jobDir in Directory.GetDirectories(FolderPath))
                {
                    DWJob job = null;
                    //Verifico che le regole di lettura della commessa siano valide 
                    foreach (var jobsFactory in jobsFactories)
                    {

                        job = jobsFactory.CreateJobFromFolderName(jobDir);

                        if (job != null)
                        {
                            job.JobId = i;
                            break;
                        }
                    }

                    if (job != null)
                    {
                        cachedJobs.Add(i, job);
                        i++;
                    }
                }
            }

            return cachedJobs.Values.ToList();
        }

        public List<string> GetJobCategories(int jobId)
        {
            AssertFolderExists();
            var job = GetJobs().FirstOrDefault(j => jobId == j.JobId);
            if (job != null)
                throw new DAStoresException($"La commessa non è valorizzata");

            List<string> results = new List<string>();

            foreach (var jobsFactory in jobsFactories)
            {
                var jobFolderName = jobsFactory.CreateFolderNameFromJob(job);
                if (!string.IsNullOrWhiteSpace(jobFolderName))
                {
                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(FolderPath, jobFolderName));
                    if (dir != null)
                    {
                        foreach (var subdir in dir.GetDirectories())
                        {
                            if (File.Exists(Path.Combine(subdir.FullName, DAFileFolderStore.CATEGORY_INFO_JSON_FILE)))
                                results.Add(subdir.Name);
                        }
                        break;
                    }
                }
            }
            return results;
        }

        public bool AddJob(DWJob job)
        {
            AssertFolderExists();

            //Verifico se esiste una commessa con lo stesso id
            AssertJobNotExists(job);

            foreach (var jobFactory in jobsFactories)
            {
                var jobDirectoryName = jobFactory.CreateFolderNameFromJob(job);
                if (!string.IsNullOrWhiteSpace(jobDirectoryName))
                {
                    Directory.CreateDirectory(Path.Combine(FolderPath, jobDirectoryName));
                    return true;
                }
            }

            return false;
        }


        public DWJob GetJob(int jobId)
        {
            var job = GetJobs().FirstOrDefault(j => jobId == j.JobId);

            return job;
        }
        #endregion

        #region Private functions

        private void AssertJobNotExists(DWJob job)
        {
            var foundJobs = from j in GetJobs()
                            where j.JobId == job.JobId
                            select j;
            if (foundJobs.Count() > 0)
                throw new DAStoresException($"Esiste già una commessa con codice {job.JobId}");
        }


        private void AssertFolderExists()
        {
            if (string.IsNullOrWhiteSpace(FolderPath))
            {
                throw new DAStoresException($"Percorso per il recupero delle commesse non impostato");
            }

            if (!Directory.Exists(FolderPath))
            {
                throw new DAStoresException($"Percorso '{FolderPath}' non trovato");
            }
        }

        public List<DWItem> GetJobItems(DWJob job)
        {
            List<DWItem> items = new List<DWItem>();


            foreach (var jobsFactory in jobsFactories)
            {
                string jobFolderName = jobsFactory.CreateFolderNameFromJob(job);
                var jobFolderPath = Path.Combine(FolderPath, jobFolderName);
                if (Directory.Exists(jobFolderPath))
                {

                    foreach (var subDir in Directory.GetDirectories(jobFolderPath))
                    {
                        var item = jobsFactory.CreateItemFromFoldeName(subDir);
                        if (item != null)
                            items.Add(item);



                    }
                    foreach (var itemFile in Directory.GetFiles(jobFolderPath, "item.json", SearchOption.AllDirectories))
                    {

                    }
                    break;
                }

            }

            return items;
        }

        public bool UpdateItem(DWItem item, DWJob job)
        {
            string folderPath = string.Empty;
            string itemFolderPath = string.Empty;
            return SaveItemJson(item, job, out folderPath, out itemFolderPath, false);
        }

        private bool SaveItemJson(DWItem item, DWJob job, out string jobFolderPath, out string itemFolderPath, bool checkNotExist)
        {

            jobFolderPath = string.Empty;
            itemFolderPath = string.Empty;

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            AssertFolderExists();


            foreach (var jobFactory in jobsFactories)
            {
                jobFolderPath = jobFactory.CreateFolderNameFromJob(job);
                if (!string.IsNullOrEmpty(jobFolderPath))
                {
                    item.Path = jobFactory.CreateItemFolderFromItem(item);
                    itemFolderPath = Path.Combine(Path.Combine(this.FolderPath, jobFolderPath), item.Path);
                    if (!Directory.Exists(itemFolderPath))
                        Directory.CreateDirectory(itemFolderPath);
                    else
                       if (checkNotExist) throw new DAStoresException($"Il file {job.JobId} è già stato salvato");

                    //Scrivo il file json con le informazioni
                    File.WriteAllText(Path.Combine(itemFolderPath, ITEM_INFO_JSON_FILE), JsonConvert.SerializeObject(item));
                    return true;
                }
            }
            return false;
        }

        public List<DWNote> GetJobNotes(int jobId)
        {
            var jobFolderPath = string.Empty;
            var notes = new List<DWNote>();
            AssertFolderExists();

            var job = this.GetJob(jobId);

            if (job == null)
                throw new DAJobNotFoundInStoreException(jobId.ToString());


            foreach (var jobFactory in jobsFactories)
            {
                jobFolderPath = Path.Combine(FolderPath, jobFactory.CreateFolderNameFromJob(job));
                if (!string.IsNullOrEmpty(jobFolderPath))
                {
                    string notesFilePath = Path.Combine(jobFolderPath, JOB_NOTES_JSON_FILE);
                    if (File.Exists(notesFilePath))
                        notes.AddRange(JsonConvert.DeserializeObject<DWNote[]>(File.ReadAllText(notesFilePath)));
                }
            }
            return notes;
        }

        public DWStoreInfo AddJobNoteToJob(int jobId, DWNote note)
        {

            var result = new DWStoreInfo() { Stored = false };
            result.ErrorMessages.Add("Salvataggio delle note non supportato");
            return result;
        }

        private DWStoreInfo GetStoreInfo(string message)
        {
            var storeInfo = new DWStoreInfo();
            storeInfo.ErrorMessages.Add(message);
            return storeInfo;
        }

        public List<DWNote> GetUnremindedNotes()
        {
            return new List<DWNote>();
        }

        public DWStoreInfo UpdateNote(DWNote note)
        {
            return GetStoreInfo("Lo store non gestisce la modifica della singola nota");
        }

        public DWScope GetNoteScope(DWNote note)
        {
            return new DWScope() { Item = null, Job = null };
        }

        public bool InitStore()
        {
            return true;
        }

        #endregion

        #region Sync
        public bool AlreadySync(IDASyncStore syncStore)
        {
            return true;
        }

        public bool SyncObject<T>(T obj)
        {
            return true;
        }

        public bool BeginSync(IDASyncStore store, ILogger syncLogger)
        {
            return true;
        }

        public bool CompleteSync(IDASyncStore store)
        {
            return true;
        }

        public bool SyncJob(DWJob job)
        {

            foreach (var jobsFactory in jobsFactories)
            {
                var jobFolderName = jobsFactory.CreateFolderNameFromJob(job);
                job.Path = jobFolderName;
                if (!string.IsNullOrWhiteSpace(jobFolderName))
                {
                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(FolderPath, jobFolderName));
                    if (!dir.Exists)
                        dir.Create();
                }
            };
            return true;
        }

        public bool SyncItem(DWJob job, DWItem item)
        {
            return this.Store(item, job).Stored;
        }

        public bool TryRollbackSync(IDASyncStore store)
        {
            return true;
        }

        public DWStoreInfo Store(DWTopic topic)
        {
            return new DWStoreInfo() { Stored = true };
        }

        public Task<byte[]> GetDocumentVersionData(int id)
        {
            return null;
        }

        public DWStoreInfo AddJobTag(int jobId, DWTag tag)
        {
            return new DWStoreInfo() { Stored = true };
        }


        private string GetJobExistingPath(DWJob job)
        {

            foreach (var jobFactory in jobsFactories)
            {
                var jobFolderPath = jobFactory.CreateFolderNameFromJob(job);
                if (!string.IsNullOrWhiteSpace(jobFolderPath))
                {
                    var fullPath = Path.Combine(folderPath, jobFolderPath);
                    if (Directory.Exists(fullPath))
                        return fullPath;
                }

            }
            return string.Empty;
        }

        private string GetItemExistingPath(DWJob job, DWItem item)
        {


            foreach (var jobFactory in jobsFactories)
            {
                var jobFolderPath = jobFactory.CreateFolderNameFromJob(job);
                if (!string.IsNullOrWhiteSpace(jobFolderPath))
                {
                    var fullPath = Path.Combine(folderPath, jobFolderPath, jobFactory.CreateItemFolderFromItem(item));
                    if (Directory.Exists(fullPath))
                        return fullPath;
                }

            }
            return string.Empty;
        }

        public bool SyncDeleteJob(DWJob job)
        {
            var response = new DWStoreInfo();
            try
            {
                var deleteJobFolder = GetJobExistingPath(job);
                if (!string.IsNullOrWhiteSpace(deleteJobFolder))
                {
                    Directory.Delete(deleteJobFolder, true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Errore durante la rimozione della cartella del Job {job.Code}");
            }
            return true;
        }


        public bool SyncDeleteItem(DWJob job, DWItem item)
        {
            try
            {
                var deleteJobFolder = GetJobExistingPath(job);
                if (!string.IsNullOrWhiteSpace(deleteJobFolder))
                {
                    foreach (var jobFactory in jobsFactories)
                    {
                        var itemDirectoryName = jobFactory.CreateItemFolderFromItem(item);
                        if (!string.IsNullOrWhiteSpace(itemDirectoryName))
                        {
                            Directory.Delete(Path.Combine(deleteJobFolder, itemDirectoryName), true);
                            return true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Errore durante la rimozione della cartella dell'item {item.ItemId} del job {job.Code}");
            }
            return true;
        }

        public bool SyncDeleteDocument(DWJob job, DWItem item, DWDocument document)
        {
            try
            {
                var deleteJobFolder = GetJobExistingPath(job);
                if (!string.IsNullOrWhiteSpace(deleteJobFolder))
                {
                    foreach (var jobFactory in jobsFactories)
                    {
                        var itemDirectoryName = jobFactory.CreateItemFolderFromItem(item);
                        if (!string.IsNullOrWhiteSpace(itemDirectoryName))
                        {
                            var itemPath = Path.Combine(deleteJobFolder, itemDirectoryName);
                            if (Directory.Exists(itemPath))
                            {
                                var docVersion = document.Versions.OrderByDescending(d => d.InsertDate).FirstOrDefault();
                                if (docVersion != null)
                                {
                                    string filePath = Path.Combine(itemPath, docVersion.Filename + docVersion.Extension);
                                    if (File.Exists(filePath))
                                    {
                                        File.Delete(filePath);
                                    }
                                    return true;
                                }

                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Errore durante la rimozione del documento {document.DocumentId} dell'item {item.ItemId} del Job {job.Code}");
            }
            return false;
        }

        private string GetOrCreateTagPath(DWJob job, DWTag tag)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            if (tag == null) throw new ArgumentNullException(nameof(tag));


            var jobPath = GetJobExistingPath(job);
            if (!string.IsNullOrWhiteSpace(jobPath))
            {
                int maxId = 0;
                string tagPath = string.Empty;
                foreach (var subFolder in Directory.GetDirectories(jobPath))
                {
                    var subFolderName = Path.GetFileName(subFolder);
                    if (Regex.IsMatch(subFolderName, $"[0-9][0-9][0-9]-{tag.Description.ToUpper()}"))
                    {
                        tagPath = subFolder;
                        break;
                    }
                    else if (subFolderName.Length > 4 && Regex.IsMatch(subFolderName, "[0-9][0-9][0-9]-*"))
                    {
                        var id = 0;
                        int.TryParse(subFolderName.Substring(0, 3), out id);
                        if (id > maxId)
                            maxId = id;
                    }
                }

                if (string.IsNullOrWhiteSpace(tagPath))
                {
                    tagPath = Path.Combine(jobPath, $"{maxId + 1:000}-{tag.Description.ToUpper()}");
                }

                if (!Directory.Exists(tagPath)) Directory.CreateDirectory(tagPath);

                return tagPath;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool SyncRemoveDocumemtTag(DWJob job, DWItem item, DWDocument document, DWTag tag)
        {
            try
            {
                var tagPath = GetOrCreateTagPath(job, tag);
                if (!string.IsNullOrWhiteSpace(tagPath))
                {
                    var version = document.Versions.OrderByDescending(d => d.InsertDate).FirstOrDefault();
                    if (version != null)
                    {
                        var docPath = GetDocumentTagFileFolder(job, item, tag, document, version);
                        if (RemoveLinkInFolder(docPath, version.Filename + version.Extension, true))
                        {
                            RemoveEmptySubfolder(docPath, tagPath);
                            return true;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Errore durante la rimozione del tag {tag.TagId} del documento {document.DocumentId} dell'item {item.ItemId} del Job {job.Code}");

            }
            return false;
        }

        private void RemoveEmptySubfolder(string startPath, string endPath)
        {
            //Verifico che le cartelle da eliminare siano sotto cartelle
            //di quella a cui fermarsi
            if (startPath.ToLower().IndexOf(endPath.ToLower()) == 0)
            {
                var pathToDelete = startPath.ToLower();
                while (pathToDelete != endPath.ToLower())
                {
                    if (Directory.GetFileSystemEntries(pathToDelete).Length == 0)
                    {
                        Directory.Delete(pathToDelete);
                        pathToDelete = Path.GetDirectoryName(pathToDelete);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public bool SyncRemoveItemTag(DWJob job, DWItem item, DWTag tag)
        {
            try
            {
                var tagPath = GetOrCreateTagPath(job, tag);
                if (!string.IsNullOrWhiteSpace(tagPath))
                {
                    var itemPath = GetItemExistingPath(job, item);

                    return RemoveLinkInFolder(tagPath, itemPath, false);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Errore durante la rimozione del tag {tag.TagId} dell'item {item.ItemId} del Job {job.Code}");

            }
            return false;
        }

        private bool RemoveLinkInFolder(string folderPath, string linkPath, bool deleteOnEmpty)
        {

            foreach (var link in Directory.GetFileSystemEntries(folderPath))
            {
                if (Path.GetFileName(link).ToLower() == $"{Path.GetFileName(linkPath)}.lnk".ToLower())
                {
                    File.Delete(link);
                    return true;
                }
            }

            if (deleteOnEmpty && Directory.GetFiles(folderPath).Length == 0)
            {
                Directory.Delete(folderPath);
            }

            return false;
        }


        private string GetItemTagFolder(DWJob job, DWItem item, DWTag tag)
        {
            var jobPath = GetJobExistingPath(job);
            if (!string.IsNullOrWhiteSpace(jobPath))
            {
                var jobFolder = Path.Combine(this.FolderPath, jobPath);
                foreach (var jobFactory in jobsFactories)
                {
                    var jobFolderPath = jobFactory.CreateFolderNameFromJob(job);
                    if (!string.IsNullOrEmpty(jobFolderPath))
                    {
                        var itemFolder = jobFactory.CreateItemFolderFromItem(item);

                        string tagPath = Path.Combine(GetOrCreateTagPath(job, tag), itemFolder);
                        if (!Directory.Exists(tagPath))
                            Directory.CreateDirectory(tagPath);
                        return tagPath;
                    }
                }
            }
            return string.Empty;
        }


        private string GetDocumentTagFileFolder(DWJob job, DWItem item, DWTag tag, DWDocument document, DWDocumentVersion version)
        {
            var itemFolder = GetItemTagFolder(job, item, tag);
            var versionSubFolder = Path.GetDirectoryName(version.Filename);

            var folder = Path.Combine(itemFolder, versionSubFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;

        }

        public bool SyncAddDocumentTag(DWJob job, DWItem item, DWDocument document, DWTag tag, string fileVersionPath)
        {

            var tagItemPath = GetDocumentTagFileFolder(job, item, tag, document, document.Versions.FirstOrDefault());
            if (!string.IsNullOrEmpty(tagItemPath))
            {
                CreateShortcutInPath(tagItemPath, fileVersionPath, Path.GetFileName(fileVersionPath));
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SyncAddItemTag(DWJob job, DWItem item, DWTag tag)
        {
            var jobPath = GetJobExistingPath(job);
            if (!string.IsNullOrWhiteSpace(jobPath))
            {
                foreach (var jobFactory in jobsFactories)
                {
                    var jobFolderPath = jobFactory.CreateFolderNameFromJob(job);
                    if (!string.IsNullOrEmpty(jobFolderPath))
                    {
                        var itemFolderName = jobFactory.CreateItemFolderFromItem(item);
                        var itemFolderPath = Path.Combine(Path.Combine(this.FolderPath, jobFolderPath), itemFolderName);
                        if (Directory.Exists(itemFolderPath))
                        {
                            string tagPath = GetOrCreateTagPath(job, tag);
                            CreateShortcutInPath(tagPath, itemFolderPath, Path.GetFileName(itemFolderPath));

                        }

                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
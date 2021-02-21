using Delineat.Assistant.Core.Helpers;
using Delineat.Assistant.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Delineat.Assistant.Core.Stores.Factories
{
    public class DADelineatJobsFactory : Interfaces.IDADelineatJobsFactory
    {
        private const int JOBID_VALID_LENGTH = 5;
        private readonly ILogger logger;

        public DADelineatJobsFactory(ILogger logger)
        {
            this.logger = logger;
        }
        public string CreateFolderNameFromJob(DWJob job)
        {
            if (job.Code != null && job.Code.Length == JOBID_VALID_LENGTH)
            {
                string jobId = string.Empty;
                if (TryParseJobId(job.Code, out jobId))
                {
                    if (string.IsNullOrEmpty(job.Path))
                    {
                        return $"{jobId}_{job.Description}";
                    }
                    else
                    {
                        return job.Path;
                    }
                }
            }
            return string.Empty;
        }

        private bool TryParseJobId(string value, out string jobID)
        {
            jobID = string.Empty;
            if (value.Length >= 5)
            {
                int jobNumericIdPart = 0;
                if (int.TryParse(value.Substring(0, 3), out jobNumericIdPart))
                {
                    jobID = value.Substring(0, 5);
                    return true;
                }
            }
            return false;
        }

        public DWJob CreateJobFromFolderName(string fullDirectoryPath)
        {
            if (!Directory.Exists(fullDirectoryPath))
            {
                throw new DirectoryNotFoundException(fullDirectoryPath);
            }
            //Primi 3 caratteri numerici e 2 caratteri alfanumerici compongono il codice della commessa,
            //Il resto se c'è è la descrizione
            string directoryName = Path.GetFileName(fullDirectoryPath);

            if (directoryName.Length >= 5)
            {
                string code = string.Empty;
                if (TryParseJobId(directoryName, out code))
                {
                    var job = new DWJob();
                    job.Code = code;
                    if (directoryName.Length > 5)
                    {
                        job.Description = directoryName.Substring(5);
                        if (job.Description.Length > 2 && job.Description[0] == '_')
                            job.Description = job.Description.Substring(1);
                    }

                    job.Path = directoryName;
                    return job;
                }
            }
            return null;
        }


        public string CreateItemFolderFromItem(DWItem item)
        {
            return DAFileHelper.GetSafeFilename($"{(char)item.ItemType}{item.Date:yyMMdd}_{item.Who}_{item.Description.TrimEnd()}");
        }

        public DWItem CreateItemFromFoldeName(string folderName)
        {
            DWItem item = null;
            if (Directory.Exists(folderName))
            {
                string itemFilePath = Path.Combine(folderName, "item.json");

                //Vedo se è stato salvato un item completo
                if (File.Exists(itemFilePath))
                {
                    string json = File.ReadAllText(itemFilePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        try
                        {
                            item = JsonConvert.DeserializeObject<DWItem>(json);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Impossibile creare un item dalla cartella {folderName}");
                            item = null;
                        }
                    }
                }


                if (item == null)
                {
                    string directoryName = Path.GetFileName(folderName);
                    if (IsItemDirectoryName(directoryName))
                    {
                        item = new DWItem();

                        try
                        {
                            //il primo carattere è l'
                            item.ItemType = (ItemType)directoryName.ToLower()[0];
                        }
                        catch
                        {
                            item.ItemType = ItemType.None;
                        }
                        item.Path = directoryName;
                        DateTime date = DateTime.MaxValue;
                        item.Date = DateTime.MaxValue;
                        try
                        {

                            if (directoryName.Length > 8)
                            {
                                if (DateTime.TryParseExact(directoryName.Substring(1, 8), "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                                {
                                    item.Date = date;
                                }
                            }

                            if (item.Date == DateTime.MaxValue)
                            {
                                if (directoryName.Length > 6)
                                {
                                    if (DateTime.TryParseExact($"20{directoryName.Substring(1, 6)}", "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                                    {
                                        item.Date = date;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            item.Date = DateTime.MinValue;
                        }

                        int whoIndex = directoryName.IndexOf('_') + 1;

                        int descriptionIndex = directoryName.IndexOf('_', whoIndex) + 1;
                        item.Who = "Nessun referente";
                        if (directoryName.Length >= whoIndex)
                        {
                            if (descriptionIndex > whoIndex)
                                item.Who = directoryName.Substring(whoIndex, descriptionIndex - whoIndex - 1);
                            else
                                item.Who = directoryName.Substring(whoIndex);
                        }
                        if (descriptionIndex > 0)
                        {
                            item.Description = "Nessuna descrizione";
                        }
                        if (directoryName.Length >= descriptionIndex)
                            item.Description = directoryName.Substring(descriptionIndex);

                        foreach (var file in Directory.GetFiles(folderName))
                        {
                            if (Path.GetFileName(file).ToLower() != "item.json")
                            {
                                item.Attachments.Add(new DWAttachment() { Path = file, RootPath = string.Empty });
                            }
                        }

                    }
                }
            }
            return item;
        }

        private bool IsItemDirectoryName(string directoryName)
        {
            //Formato (Tipo)(YYYYMMDD)_
            var result = Regex.IsMatch(directoryName, "[a-z,A-Z][[0-9]{8}_")
            || Regex.IsMatch(directoryName, "[a-z,A-Z][[0-9]{6}_");
            return result;
        }
    }
}

using Delineat.Assistant.API.Helpers;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.Models;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using SharpCompressWrapper_v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Delineat.Assistant.API
{
    public class DALoadingSessionManager
    {
        private readonly string sessionsFolder;
        private readonly ILogger logger;

        public DALoadingSessionManager(string sessionFolder,ILogger logger)
        {
            this.sessionsFolder = sessionFolder;
            this.logger = logger;
        }

        private string GetSessionPath(Guid sessionId, bool createIfNotExists = false)
        {

            var sessionPath = Path.Combine(sessionsFolder,
                sessionId.ToString());

            if (createIfNotExists && !Directory.Exists(sessionPath))
                Directory.CreateDirectory(sessionPath);

            return sessionPath;
        }

        public DALoadingSession CreateSession()
        {
            var newSession = new DALoadingSession();
            while (string.IsNullOrWhiteSpace(newSession.FolderPath))
            {
                newSession.Id = Guid.NewGuid();
                //Verifico se esiste già una nuova sessione con l'id
                var existingSession = this.GetSessionWithId(newSession.Id);
                //Se non esisto assegno la cartella alla nuova sessione
                if (existingSession == null)
                {
                    newSession.FolderPath = GetSessionPath(newSession.Id, true);
                }
            }
            return newSession;
        }

        public DALoadingSession GetSessionWithId(Guid id)
        {
            string sessionPath = GetSessionPath(id);
            if (Directory.Exists(sessionPath))
            {
                return new DALoadingSession() { Id = id, FolderPath = sessionPath };
            }
            else
            {
                return null;
            }
        }

        public string AddFileToSession(DALoadingSession loadingSession, string fileName, Stream data, List<string> protectedFile)
        {
            string filePath = Path.Combine(loadingSession.FolderPath, fileName);
            using (FileStream sw = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                DAStreamHelper.CopyStream(data, sw);
            }

            CheckZipFile(filePath, string.Empty, protectedFile);

            return filePath;
        }

        private void CheckZipFile(string filePath, string password, List<string> protectedFiles)
        {
            try
            {
                string zipFolder = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));


                switch (Path.GetExtension(filePath.ToLower()))
                {
                    case ".zip":
                        using (ZipFile archive = new ZipFile(filePath))
                        {
                            archive.Password = password;
                            if (archive.TestArchive(true))
                            {
                                FastZip zip = new FastZip();
                                zip.Password = password;
                                zip.ExtractZip(filePath, zipFolder, string.Empty);
                            }
                            else
                            {
                                protectedFiles.Add(filePath);
                            }
                        }
                        break;

                    case ".rar":
                        ZipExtractor.ExtractAll(filePath, zipFolder);
                        break;


                    case ".7z":
                    case ".tar":
                        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "x64" : "x86", "7z.dll");
                        SevenZip.SevenZipBase.SetLibraryPath(path);
                        using (var extractor = new SevenZip.SevenZipExtractor(filePath, password))
                        {
                            if (extractor.Check())
                            {
                                extractor.ExtractArchive(zipFolder);
                            }
                            else
                            {
                                protectedFiles.Add(filePath);
                            }
                        }
                        break;
                }
                if (Directory.Exists(zipFolder))
                {
                    foreach (var file in Directory.GetFiles(zipFolder, "*.*", SearchOption.AllDirectories).ToList())
                    {
                        CheckZipFile(file, password, protectedFiles);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossibile decomprimere il file {filePath}");
            }

        }


        internal bool ExtractFileWithPassword(DALoadingSession loadingSession, DWFilePassword item, List<string> protectedFiles)
        {
            CheckZipFile(item.Path, item.Password, protectedFiles);
            return protectedFiles.Count == 0;
        }
    }
}

using Delineat.Assistant.API.Models;
using Delineat.Assistant.Core.Tips.Interfaces;
using System;
using System.IO;

namespace Delineat.Assistant.API.Managers
{
    public class DASessionTipsAttachmentsStoreManager : IDWTipsAttachmentsStore
    {
        private DALoadingSession loadingSession;

        public DASessionTipsAttachmentsStoreManager(DALoadingSession loadingSession)
        {
            this.loadingSession = loadingSession;
        }

        public bool StoreFile(string fileName, byte[] data)
        {
            try
            {
                File.WriteAllBytes(Path.Combine(loadingSession.FolderPath, fileName), data);
            }
            catch (Exception)
            {

            }
            return true;
        }
    }
}

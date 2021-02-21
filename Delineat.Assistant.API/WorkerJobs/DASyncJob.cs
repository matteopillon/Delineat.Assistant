using Delineat.Assistant.API.Managers;
using Delineat.Assistant.Core.Stores.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Delineat.Assistant.API.WorkerJobs
{
    public class DASyncJob : IDAWorkerJob
    {
        private readonly DAStoresConfiguration storeConfiguration;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<DASyncJob> logger;

        public DASyncJob(DAStoresConfiguration storeConfiguration, ILoggerFactory loggerFactory)
        {
            this.storeConfiguration = storeConfiguration;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<DASyncJob>();
        }

        public bool Execute()
        {
            try
            {
                DAStoresManager syncManager = new DAStoresManager(loggerFactory, storeConfiguration);
                syncManager.SyncStores();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Errore nella sincronizzazione degli store");
                return false;
            }
        }
    }
}

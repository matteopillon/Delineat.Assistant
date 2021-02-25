using Delineat.Assistant.API.Managers;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Delineat.Assistant.API.WorkerJobs
{
    public class DASyncJob : IDAWorkerJob
    {
        private readonly IOptions<DAStoresConfiguration> storeConfiguration;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<DASyncJob> logger;
        private readonly IDAStore store;

        public DASyncJob(IOptions<DAStoresConfiguration> storeConfiguration, ILoggerFactory loggerFactory, IDAStore store)
        {
            this.storeConfiguration = storeConfiguration;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<DASyncJob>();
            this.store = store;
        }

        public bool Execute()
        {
            try
            {
                DAStoresManager syncManager = new DAStoresManager(loggerFactory, storeConfiguration, store);
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

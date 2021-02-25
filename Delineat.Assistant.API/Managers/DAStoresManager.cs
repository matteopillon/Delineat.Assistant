using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores;
using Delineat.Assistant.Core.Stores.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Delineat.Assistant.API.Managers
{

    public class DAStoresManager
    {

        public DAStoresManager(ILoggerFactory loggerFactory, IOptions<DAStoresConfiguration> storeConfiguration, IDAStore store)
        {
            this.logger = loggerFactory.CreateLogger<DAStoresManager>();
            this.store = store;
            this.configuration = storeConfiguration.Value;
        }

        private List<IDAStore> cachedStores = new List<IDAStore>();
        private readonly ILogger logger;
        private readonly IDAStore store;
        private readonly DAStoresConfiguration configuration;

        internal void ClearCache()
        {
            cachedStores = new List<IDAStore>(); ;
        }

        public void InitStores()
        {
            logger.LogTrace("Inizializzazione stores");

            store.InitStore();

            SyncStores();
        }

        public void SyncStores()
        {

            var syncManager = new DAStoreSyncManager(store, logger);
            syncManager.SyncFromSource();

        }
    }
}

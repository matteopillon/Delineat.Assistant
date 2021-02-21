using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores;
using Delineat.Assistant.Core.Stores.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Delineat.Assistant.API.Managers
{

    public class DAStoresManager
    {

        public DAStoresManager(ILoggerFactory loggerFactory, DAStoresConfiguration storeConfiguration)
        {
            this.logger = loggerFactory.CreateLogger<DAStoresManager>();
            this.configuration = storeConfiguration;
        }

        private List<IDAStore> cachedStores = new List<IDAStore>();
        private readonly ILogger logger;
        private readonly DAStoresConfiguration configuration;

        public List<IDAStore> GetStores()
        {
            lock (cachedStores)
            {
                if (cachedStores != null && cachedStores.Count == 0)
                {
                    var storeFactory = new Core.Stores.Factories.DAConfigurationStoresFactory(configuration,logger);
                    cachedStores = storeFactory.CreateStores();

                }
            }
            return cachedStores;
        }

        internal void ClearCache()
        {
            cachedStores = new List<IDAStore>(); ;
        }

        public void InitStores()
        {
            logger.LogTrace("Inizializzazione stores");
            var stores = GetStores();
            foreach (var store in stores)
            {
                store.InitStore();
            }
            SyncStores();
        }

        public void SyncStores()
        {
            var stores = GetStores();
            foreach (var store in stores)
            {
                var syncManager = new DAStoreSyncManager(store, logger);
                syncManager.SyncFromSource();
            }
        }
    }
}

using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Stores.Intefaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Stores.Factories
{
    public class DAConfigurationDBStoreFactory : IDAStoreFactory
    {
        private readonly DAStoresConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;
        private readonly DAAssistantDBContext dbContext;
        private readonly ILogger<DAConfigurationDBStoreFactory> logger;

        public DAConfigurationDBStoreFactory(DAAssistantDBContext dbContext, DAStoresConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
            this.dbContext = dbContext;
            this.logger = loggerFactory.CreateLogger<DAConfigurationDBStoreFactory>();
        }

        public IDAStore MakeStore()
        {
            var dbStore = new DADBFolderStore(loggerFactory.CreateLogger<DADBFolderStore>(), dbContext);
            dbStore.SyncStores = CreateSyncStores();
            return dbStore;
        }

        public List<IDASyncStore> CreateSyncStores()
        {
            var stores = new List<IDASyncStore>();

            if (configuration.Stores != null)
            {
                foreach (var storeElement in configuration.Stores)
                {

                    try
                    {
                        var store = GetSyncStoreFromConfiguration(storeElement);
                        stores.Add(store);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Impossibile aggiungere lo store {storeElement.Name}");
                    }

                }
            }

            return stores;

        }



        private IDASyncStore GetSyncStoreFromConfiguration(DAStoreConfiguration storeElement)
        {
            Type storeType = Type.GetType(storeElement.StoreTypeName);
            IDASyncStore store = Activator.CreateInstance(storeType, logger) as IDASyncStore;
            store.Name = storeElement.Name;
            store.Description = storeElement.Description;


            var settings = new Dictionary<string, string>();
            //Check if exists a configuration for the store
            if (storeElement.Settings != null)
            {
                foreach (var setting in storeElement.Settings)
                {
                    settings.Add(setting.Key, setting.Value);
                }
            }
            store.LoadSettings(settings);

            return store;
        }
    }
}

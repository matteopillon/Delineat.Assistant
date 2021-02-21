using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Stores.Intefaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Stores.Factories
{
    public class DAConfigurationStoresFactory : IDAStoresFactory
    {
        private readonly DAStoresConfiguration configuration;
        private readonly ILogger logger;

        public DAConfigurationStoresFactory(DAStoresConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        /// <summary>
        /// Create Stores from app.config
        /// </summary>
        /// <returns></returns>
        public List<IDAStore> CreateStores()
        {
            List<IDAStore> stores = new List<IDAStore>();

            if (configuration.Stores != null)
            {
                foreach (var storeElement in configuration.Stores)
                {

                    try
                    {
                        IDAStore store = GetStoreFromConfiguration(storeElement);
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

        private IDAStore GetStoreFromConfiguration(DAStoreConfiguration storeElement)
        {
            Type storeType = Type.GetType(storeElement.StoreTypeName);
            IDAStore store = Activator.CreateInstance(storeType, logger) as IDAStore;
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
            store.SyncStores = new List<IDASyncStore>();
            if (storeElement.SyncStores != null)
            {
                foreach (var syncStoreConfiguration in storeElement.SyncStores)
                {
                    var syncStore = GetSyncStoreFromConfiguration(syncStoreConfiguration);
                    store.SyncStores.Add(syncStore);
                }
            }

            return store;
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

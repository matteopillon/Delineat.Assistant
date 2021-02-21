using System.Collections.Generic;

namespace Delineat.Assistant.Core.Stores.Configuration
{
    public class DAStoreConfiguration
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string StoreTypeName
        {
            get;
            set;
        }

        public List<DAStoreSettingsConfiguration> Settings
        {
            get;
            set;
        }

        public List<DAStoreConfiguration> SyncStores
        {
            get;
            set;
        }

    }
}

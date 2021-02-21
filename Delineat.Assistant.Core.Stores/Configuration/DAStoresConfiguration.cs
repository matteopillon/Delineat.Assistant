using System.Collections.Generic;

namespace Delineat.Assistant.Core.Stores.Configuration
{
    public class DAStoresConfiguration
    {
        public DAStoresConfiguration()
        {
            Stores = new List<DAStoreConfiguration>();
        }

        public List<DAStoreConfiguration> Stores { get; set; }
    }
}

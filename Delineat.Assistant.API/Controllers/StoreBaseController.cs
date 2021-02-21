using Delineat.Assistant.API.Exceptions;
using Delineat.Assistant.API.Managers;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Controllers
{
    public class StoreBaseController : BaseController
    {
        private readonly IOptions<DAStoresConfiguration> storeConfiguration;
       

        public StoreBaseController(Microsoft.Extensions.Options.IOptions<DAStoresConfiguration> storesConfiguration, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            this.storeConfiguration = storesConfiguration;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<StoreBaseController>();
        }


        protected List<IDAStore> GetStores()
        {
            var stores = new DAStoresManager(loggerFactory, this.storeConfiguration.Value).GetStores();
            if (stores == null || stores.Count == 0)
            {
                throw new DAApplicationException("Nessuno store configurato nel servizio");
            }
            return stores;
        }
    }
}

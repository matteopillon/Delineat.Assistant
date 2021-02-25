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
         private readonly IDAStore store;

        public StoreBaseController(IDAStore store, ILogger logger) : base(logger)
        {
            this.store = store;
        }

        protected IDAStore Store
        {
            get
            {
                return store;
            }
        }
    }
}

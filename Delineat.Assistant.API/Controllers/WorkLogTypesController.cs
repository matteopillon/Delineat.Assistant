using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Stores.Exceptions;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkLogTypesController : StoreBaseController
    {
        public WorkLogTypesController(IOptions<DAStoresConfiguration> storesConfiguration, ILoggerFactory loggerFactory) : base(storesConfiguration, loggerFactory)
        {
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<WorkLogTypesController>();
        }

        [HttpGet()]
        public ActionResult<DWWorkLogType[]> GetWorkLogTypes()
        {
           
            try
            {
                var stores = GetStores();
                foreach (var store in stores)
                {
                    return store.GetWorkLogTypes().ToArray();
                }

                return new DWWorkLogType[0];
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

              
    }
}

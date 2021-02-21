using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : StoreBaseController
    {
        public TagsController(Microsoft.Extensions.Options.IOptions<DAStoresConfiguration> storesConfiguration,
            ILoggerFactory loggerFactory) : base(storesConfiguration, loggerFactory)
        {
            
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<TagsController>();
        }

        [HttpGet]
        public ActionResult<DWTag[]> GetTags()
        {          
            try
            {
                var tags = new List<DWTag>();
                var stores = GetStores();
                foreach (var store in stores)
                {
                    tags.AddRange(store.GetTags());
                }
                return tags.ToArray();
               
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }

    }
}

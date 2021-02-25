using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Interfaces;
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
        public TagsController(IDAStore store,
            ILogger<TagsController> logger) : base(store, logger)
        {

        }

        [HttpGet]
        public ActionResult<DWTag[]> GetTags()
        {
            try
            {
                var tags = new List<DWTag>();

                tags.AddRange(Store.GetTags());

                return tags.ToArray();

            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }

    }
}

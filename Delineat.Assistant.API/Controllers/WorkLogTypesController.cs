using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkLogTypesController : StoreBaseController
    {
        public WorkLogTypesController(IDAStore store, ILogger<WorkLogTypesController> logger) : base(store, logger)
        {
        }

        [HttpGet()]
        public ActionResult<DWWorkLogType[]> GetWorkLogTypes()
        {
            try
            {
                return Store.GetWorkLogTypes().ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }


    }
}

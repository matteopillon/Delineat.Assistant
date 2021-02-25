using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Exceptions;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkLogsController : StoreBaseController
    {
        public WorkLogsController(IDAStore store, ILogger<WorkLogsController> logger) : base(store, logger)
        {
        }


        [HttpPut("{id}")]
        public ActionResult<DWWorkLog> SaveWorkLog(int id, DWWorkLog workLog)
        {
            try
            {
                var storeInfo = Store.UpdateWorkLog(workLog);

                if (storeInfo.Stored)
                    return storeInfo.Data;
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, storeInfo.ErrorMessages);

            }
            catch (DAJobNotFoundInStoreException ex)
            {

                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpPost("{id}/tags")]

        public ActionResult<DWTag[]> AddDocumentTags(int id, DWTag[] tags)
        {

            try
            {

                DAValidationResult validation = null;
                foreach (var tag in tags)
                {
                    validation = new DAModelValidator(Store).Validate(tag);
                    if (!validation.IsValid) break;
                }

                if (validation.IsValid)
                {
                    var result = Store.AddDocumentTags(id, tags);
                    if (result.Stored)
                        return result.Data;
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessages);

                }
                else
                {
                    return BadRequest(validation.Errors);
                }


            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }
    }
}

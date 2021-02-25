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
    public class DocumentsController : StoreBaseController
    {
        public DocumentsController(IDAStore store, ILogger<DocumentsController> logger) : base(store, logger)
        {
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteDocument(int id)
        {
            try
            {
                var storeInfo = Store.DeleteDocument(id);
                if (storeInfo.Stored)
                    return Ok();
                else
                {
                    return NotFound();
                }
            }
            catch (DAJobNotFoundInStoreException)
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

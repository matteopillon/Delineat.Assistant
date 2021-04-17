using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Stores.Exceptions;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : StoreBaseController
    {
        private readonly DAServerConfiguration serverConfiguration = new DAServerConfiguration();

        public ItemsController(IDAStore store,
            IOptions<DAServerConfiguration> serverConfigurationOptions, ILogger<ItemsController> logger) : base(store, logger)
        {
            if (serverConfigurationOptions != null)
                serverConfiguration = serverConfigurationOptions.Value;
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteItem(int jobId, int id)
        {

            try
            {
                var storeInfo = Store.DeleteItem(id);
                if (storeInfo.Stored)
                {
                    return Ok();
                }
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


        [HttpDelete("{itemId}/tags/{id}")]
        public ActionResult DeleteItemTag(int itemId, int id)
        {


            try
            {
                var storeInfo = Store.RemoveItemTag(itemId, id);

                if (storeInfo.Stored)
                {
                    return Ok();
                }
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
        public ActionResult<DWTag[]> AddItemTags(int id, DWTag[] tags)
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
                    var result = Store.AddItemTags(id, tags);
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


        [HttpPost("{id}/worklogs")]
        public ActionResult<DWWorkLog> AddItemWorkLog(int id, DWWorkLog log)
        {
            try
            {


                DAValidationResult validation = new DAModelValidator(Store).Validate(log);

                if (validation.IsValid)
                {
                    var result = Store.AddWorkLogToItem(id, log);
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

        [HttpPost()]
        public ActionResult<DWItem> StoreItem(DAStoreItemRequest request)
        {
            DALoadingSessionManager manager = new DALoadingSessionManager(serverConfiguration.SessionsPath, this.logger);
            foreach (var loadingSessionId in request.LoadingSessions)
            {
                var loadingSession = manager.GetSessionWithId(loadingSessionId);
                if (loadingSession != null)
                {
                    foreach (var file in Directory.GetFiles(loadingSession.FolderPath, "*.*", SearchOption.AllDirectories))
                    {
                        request.Item.Attachments.Add(new DWAttachment() { Path = file, RootPath = loadingSession.FolderPath });
                    }
                }
            }

            foreach (var note in request.Item.Notes)
            {
                note.NoteType = NoteType.Item;
                note.InsertDate = DateTime.Now;
            }


            try
            {
                var job = Store.GetJob(request.Item.JobId);
                var storeInfo = Store.Store(request.Item, job);

                int itemId = storeInfo.Data;
                DWItem itemDetail = null;
                if (itemId > 0)
                {
                    itemDetail = Store.GetItem(itemId);
                }

                if (storeInfo.Stored)
                {
                    return itemDetail;
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, storeInfo.ErrorMessages);
                }

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
    }
}

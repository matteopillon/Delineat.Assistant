using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
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

        public ItemsController(Microsoft.Extensions.Options.IOptions<DAStoresConfiguration> storesConfiguration,
            IOptions<DAServerConfiguration> serverConfigurationOptions, ILoggerFactory loggerFactory) : base(storesConfiguration, loggerFactory)
        {
            if (serverConfigurationOptions != null)
                serverConfiguration = serverConfigurationOptions.Value;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<ItemsController>();
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteItem(int jobId, int id)
        {

            try
            {
                var stores = GetStores();
                foreach (var store in stores)
                {
                    try
                    {
                        var storeInfo = store.DeleteItem(id);
                        if (storeInfo.Stored)
                        {
                            return Ok();
                        }

                    }
                    catch (DAJobNotFoundInStoreException)
                    {
                        if (stores.Last() == store)
                            return NotFound();
                    }
                    catch
                    {
                        throw;
                    }
                }
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
                var stores = GetStores();
                foreach (var store in stores)
                {
                    try
                    {
                        var storeInfo = store.RemoveItemTag(itemId, id);

                        if (storeInfo.Stored)
                        {
                            return Ok();
                        }
                    }
                    catch (DAJobNotFoundInStoreException)
                    {
                        if (stores.Last() == store)
                            return NotFound();
                    }
                    catch
                    {
                        throw;
                    }

                }
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
                var stores = GetStores();
                foreach (var store in stores)
                {
                    DAValidationResult validation = null;
                    foreach (var tag in tags)
                    {
                        validation = new DAModelValidator(store).Validate(tag);
                        if (!validation.IsValid) break;
                    }

                    if (validation.IsValid)
                    {
                        var result = store.AddItemTags(id, tags);
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
                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }

        [HttpPost("{id}/notes")]
        public ActionResult AddItemNote(int id, DWNote note)
        {
            try
            {
                note.NoteType = NoteType.Item;
                var stores = GetStores();
                foreach (var store in stores)
                {
                    var result = store.AddNoteToItem(id, note);
                    if (result.Stored)
                    {
                        return Ok();                      
                    }
                    else
                    {
                        return BadRequest(result.ErrorMessages);
                    }
                }

                return NotFound();
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

                var stores = GetStores();
                foreach (var store in stores)
                {
                    DAValidationResult validation = new DAModelValidator(store).Validate(log);

                    if (validation.IsValid)
                    {
                        var result = store.AddWorkLogToItem(id, log);
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
                return NotFound();
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
                var stores = GetStores();
                foreach (var store in stores)
                {
                    try
                    {
                        var job = store.GetJob(request.Item.JobId);
                        var storeInfo = store.Store(request.Item, job);

                        int itemId = storeInfo.Data;
                        DWItem itemDetail = null;
                        if (itemId > 0)
                        {
                            itemDetail = store.GetItem(itemId);
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
                        if (stores.Last() == store)
                            return NotFound();
                    }
                    catch
                    {
                        throw;
                    }

                }
                return NotFound();
            }
            catch (Exception ex)
            {
               return Problem(ex);
            }
           
        }
    }
}

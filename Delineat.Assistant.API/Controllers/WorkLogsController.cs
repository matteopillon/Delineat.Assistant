﻿using Delineat.Assistant.API.Validators;
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
    public class WorkLogsController : StoreBaseController
    {
        public WorkLogsController(IOptions<DAStoresConfiguration> storesConfiguration, ILoggerFactory loggerFactory) : base(storesConfiguration, loggerFactory)
        {
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<WorkLogsController>();
        }

        [HttpPut("{id}")]
        public ActionResult<DWWorkLog> SaveWorkLog(int id, DWWorkLog workLog)
        {
            try
            {
                var stores = GetStores();
                foreach (var store in stores)
                {
                    try
                    {
                        var storeInfo = store.UpdateWorkLog(workLog);

                        if (storeInfo.Stored)
                            return storeInfo.Data;
                        else
                            return StatusCode(StatusCodes.Status500InternalServerError, storeInfo.ErrorMessages);

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

        [HttpPost("{id}/tags")]

        public ActionResult<DWTag[]> AddDocumentTags(int id, DWTag[] tags)
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
                        var result = store.AddDocumentTags(id, tags);
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
    }
}
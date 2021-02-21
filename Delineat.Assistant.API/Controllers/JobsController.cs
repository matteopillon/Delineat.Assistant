using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
    public class JobsController : StoreBaseController
    {
        private readonly IMemoryCache memoryCache;
        private const string kJobsCacheKey = "JOBS_CACHE_KEY";
        public JobsController(Microsoft.Extensions.Options.IOptions<DAStoresConfiguration> storesConfiguration,
            ILoggerFactory loggerFactory, IMemoryCache memoryCache) : base(storesConfiguration, loggerFactory)
        {
            this.memoryCache = memoryCache;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<JobsController>();
        }

        [HttpGet]
        public ActionResult<DWJob[]> GetJobs()
        {

            try
            {
                var jobs = memoryCache?.Get(kJobsCacheKey) as List<DWJob> ?? new List<DWJob>();
                if (jobs.Count == 0)
                {
                    var stores = GetStores();
                    foreach (var store in stores)
                    {
                        jobs.AddRange(store.GetJobs());
                    }
                    memoryCache?.Set(nameof(DWJob), jobs);
                }
                return jobs.ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<DWJob> GetJob(int id)
        {

            try
            {
                var stores = GetStores();

                foreach (var store in stores)
                {
                    var job = store.GetJob(id);
                    if (job != null)
                    {
                        job.Items.AddRange(store.GetJobItems(id));
                        job.Notes.AddRange(store.GetJobNotes(id));
                        return job;
                    }
                }

                return NotFound();

            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteJob(int id)
        {

            try
            {
                var stores = GetStores();
                foreach (var store in stores)
                {
                    if (store.DeleteJob(id).Stored)
                    {
                        //Tolgo dalla cache, dovrà essere ricaricata
                        memoryCache?.Remove(kJobsCacheKey);
                        return Ok();
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }

        [HttpPut("{id}")]
        public ActionResult<DWJob> SaveJob(int? id, [FromBody] DWJob job)
        {

            try
            {

                var stores = GetStores();
                foreach (var store in stores)
                {
                    var validation = new DAModelValidator(store).Validate(job);
                    if (validation.IsValid)
                    {
                        var result = store.Store(job);
                        //Tolgo dalla cache, dovrà essere ricaricata
                        memoryCache?.Remove(kJobsCacheKey);
                        if (result.Stored)
                            return job;
                        else
                            return BadRequest(result.ErrorMessages);

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


        [Route("{jobId}/tags/{id}")]
        [HttpPut()]
        public ActionResult<DWTag> SaveJobTag(int jobId, int id, [FromBody] DWTag tag)
        {
            try
            {

                var stores = GetStores();
                foreach (var store in stores)
                {
                    var validation = new DAModelValidator(store).Validate(tag);
                    if (validation.IsValid)
                    {
                        var result = store.AddJobTag(jobId, tag);
                        if (result.Stored)
                            return tag;
                        else
                            return BadRequest(result.ErrorMessages);
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


        [Route("{jobId}/notes/{id}")]
        [HttpPut()]
        public ActionResult<DWNote> SaveJobNote(int jobId, int id, [FromBody] DWNote note)
        {
            try
            {
                note.NoteType = NoteType.Job;
                var stores = GetStores();
                foreach (var store in stores)
                {
                    var result = store.AddNoteToJob(jobId, note);

                    if (result.Stored)
                    {
                        return note;
                    }
                    else
                        return BadRequest(result.ErrorMessages);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpGet("{id}/items")]
        public ActionResult<DWItem[]> GetJobItems(int id)
        {
            try
            {
                var stores = GetStores();
                foreach (var store in stores)
                {
                    var job = store.GetJob(id);
                    if (job != null)
                    {
                        var jobItems = store.GetJobItems(id);

                        return jobItems.ToArray();
                    }
                }
                return new DWItem[0];
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(DAConsts.Logs.LoggerExceptionEventId), ex, ex.Message);
                return Problem(ex);
            }
        }
    }
}

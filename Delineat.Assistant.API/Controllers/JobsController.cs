using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Interfaces;
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
        public JobsController(IDAStore store,
            ILogger<JobsController> logger, IMemoryCache memoryCache) : base(store, logger)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public ActionResult<DWJob[]> GetJobs()
        {

            try
            {
                var jobs = memoryCache?.Get(kJobsCacheKey) as List<DWJob> ?? new List<DWJob>();
                if (jobs.Count == 0)
                {
                    jobs.AddRange(Store.GetJobs());

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

                var job = Store.GetJob(id);
                if (job != null)
                {
                    job.Items.AddRange(Store.GetJobItems(id));
                    job.Notes.AddRange(Store.GetJobNotes(id));
                    return job;
                }
                else
                {
                    return NotFound();
                }

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

                if (Store.DeleteJob(id).Stored)
                {
                    //Tolgo dalla cache, dovrà essere ricaricata
                    memoryCache?.Remove(kJobsCacheKey);
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
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


                var validation = new DAModelValidator(Store).Validate(job);
                if (validation.IsValid)
                {
                    var result = Store.Store(job);
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
                var validation = new DAModelValidator(Store).Validate(tag);
                if (validation.IsValid)
                {
                    var result = Store.AddJobTag(jobId, tag);
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

                var result = Store.AddNoteToJob(jobId, note);

                if (result.Stored)
                {
                    return note;
                }
                else
                    return BadRequest(result.ErrorMessages);

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

                var job = Store.GetJob(id);
                if (job != null)
                {
                    var jobItems = Store.GetJobItems(id);

                    return jobItems.ToArray();
                }
                else
                {
                    return new DWItem[0];
                }
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(DAConsts.Logs.LoggerExceptionEventId), ex, ex.Message);
                return Problem(ex);
            }
        }
    }
}

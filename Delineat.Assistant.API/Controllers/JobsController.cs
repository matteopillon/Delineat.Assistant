using Delineat.Assistant.API.Helpers;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Delineat.Assistant.Core.Interfaces;
using Delineat.Assistant.Core.ObjectFactories;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly DADWObjectFactory dwObjectFactory;

        public DAAssistantDBContext assistantDBContext { get; }


        public JobsController(DAAssistantDBContext assistantDBContext, IDAStore store,
            ILogger<JobsController> logger, IMemoryCache memoryCache) : base(store, logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
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

        private bool Validate(DWJob job)
        {
            var validation = new DAModelValidator(Store).Validate(job);
            if (!validation.IsValid)
            {
                foreach (var message in validation.Errors)
                {
                    ModelState.AddModelError(nameof(job), message);
                }
            }
            return ModelState.IsValid;
        }

        private bool Validate(DAJobRequest data, int? id = null)
        {
            if (data == null) ModelState.AddModelError(nameof(data), "Commessa non valorizzato");
            if (string.IsNullOrWhiteSpace(data.Code))
            {
                ModelState.AddModelError(nameof(data.Code), "Codice commessa non valorizzato");
            }
            else
            {
                // Verificare esistenza codice commessa?
            }
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                ModelState.AddModelError(nameof(data.Description), "Descrizione commessa non valorizzata");
            }
            if (data.CustomerId == 0)
            {
                ModelState.AddModelError(nameof(Job.Customer), "Cliente commessa non valorizzato");
            }
            else
            {
                //Verifico esistenza del cliente
                var customer = assistantDBContext.Customers.FirstOrDefault(c => c.CustomerId == data.CustomerId);
                if (customer == null)
                {
                    ModelState.AddModelError(nameof(Job.Customer), $"Cliente con id ${data.CustomerId} non trovato");
                }
            }



            return ModelState.IsValid;
        }
        private Job FillFromRequest(Job job, DAJobRequest data)
        {

            job.Code = data.Code;
            job.Customer = data.CustomerId > 0 ? assistantDBContext.GetCustomer(data.CustomerId) : null;
            job.Description = data.Description;
            job.CustomerInfo = data.Description;
            job.OrderRef = data.OrderRef;
            job.QuotationRef = data.QuotationRef;

            return job;
        }
        [HttpPut("{id}")]
        public ActionResult<DWJob> SaveJob(int id, DAJobRequest job)
        {

            try
            {
                if (Validate(job))
                {
                    var dbJob = assistantDBContext.GetJob(id);
                    if (dbJob != null)
                    {
                        var dwJob = this.dwObjectFactory.GetDWJob(FillFromRequest(dbJob, job));
                        var validation = new DAModelValidator(Store).Validate(dwJob);
                        if (validation.IsValid)
                        {
                            var result = Store.Store(dwJob);
                            //Tolgo dalla cache, dovrà essere ricaricata
                            memoryCache?.Remove(kJobsCacheKey);
                            if (result.Stored)
                                return dwJob;
                            else
                            {
                                foreach (var message in result.ErrorMessages)
                                {
                                    ModelState.AddModelError(nameof(Job), message);
                                }
                                return BadRequest(ModelState);
                            }


                        }
                        else
                        {
                            return BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }


        [HttpPost()]
        public ActionResult<DWJob> InsertJob(DAJobRequest job)
        {

            try
            {
                if (Validate(job))
                {

                    var dwJob = this.dwObjectFactory.GetDWJob(FillFromRequest(new Job(), job));
                    var validation = new DAModelValidator(Store).Validate(dwJob);
                    if (validation.IsValid)
                    {
                        var result = Store.Store(dwJob);
                        //Tolgo dalla cache, dovrà essere ricaricata
                        memoryCache?.Remove(kJobsCacheKey);
                        if (result.Stored)
                            return dwJob;
                        else
                        {
                            foreach (var message in result.ErrorMessages)
                            {
                                ModelState.AddModelError(nameof(Job), message);
                            }
                            return BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        return BadRequest(ModelState);
                    }

                }
                else
                {
                    return BadRequest(ModelState);
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

        [HttpGet("jobs/{jobId}/subjobs")]
        public ActionResult<DWSubJob[]> GetSubJobs(int jobId)
        {
            try
            {
                var subJobs = assistantDBContext.SubJobs
                    .Include(sj => sj.Job).ThenInclude(j => j.Customer)
                    .Include(sj => sj.Customer)
                    .Where(sj => sj.Job.JobId == jobId)
                    .Select(sj => dwObjectFactory.GetDWSubJob(sj));
                return subJobs.ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        private IQueryable<DayWorkLog> MakeDayWorkLogsQuery()
        {
            return assistantDBContext.DayWorkLogs.Include(w => w.Job)
                .Include(w => w.User)
                .Include(w => w.WorkType)
                .Include(w => w.SubJob);
        }

        private IQueryable<DayWorkLog> MakeDayWorkLogsQueryWithDateRange(DateTime startDate, DateTime endDate, int jobId)
        {
            return MakeDayWorkLogsQuery()
                .Where(d => d.Date.Date >= startDate && d.Date.Date <= endDate && d.Job.JobId == jobId).OrderBy(d=>d.Date);
        }

        [HttpGet("{jobId}/dayworklogs")]
        public ActionResult<DWDayWorkLog[]> GetWorkLogs(DateTime startDate, DateTime endDate, int jobId)
        {
            try
            {               
                return MakeDayWorkLogsQueryWithDateRange(startDate, endDate, jobId).Select(d => dwObjectFactory.GetDWDayWorkLog(d)).ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }
    }
}

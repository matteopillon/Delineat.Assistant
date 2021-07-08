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
        private readonly DADataObjectFactory dataObjectFactory;

        public DAAssistantDBContext assistantDBContext { get; }


        public JobsController(DAAssistantDBContext assistantDBContext, IDAStore store,
            ILogger<JobsController> logger, IMemoryCache memoryCache) : base(store, logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.dataObjectFactory = new DADataObjectFactory(assistantDBContext);
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

        private bool Validate(DAAddJobRequest data)
        {


            if (data.ParentId.HasValue)
            {
                var parentJob = assistantDBContext.GetJob(data.ParentId.Value);
                if (parentJob == null)
                {
                    ModelState.AddModelError(nameof(data.ParentId), "Commessa padre non trovata");
                }
            }

            ValidateBase(data.Job, data.ParentId);

            return ModelState.IsValid;
        }


        private bool ValidateBase(DAJobRequest data, int? parentId = null)
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

            if (!data.BeginDate.HasValue)
            {
                ModelState.AddModelError(nameof(data.BeginDate), "Data inizio commessa non valorizzata");
            }

            if (string.IsNullOrWhiteSpace(data.Description))
            {
                ModelState.AddModelError(nameof(data.Description), "Descrizione commessa non valorizzata");
            }
            if (data.CustomerId == 0)
            {
                if (!parentId.HasValue)
                {
                    ModelState.AddModelError(nameof(Job.Customer), "Cliente commessa non valorizzato");
                }
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
            if (data.BeginDate.HasValue)
            {
                if (data.BeginDate.Value.Kind == DateTimeKind.Utc)
                {
                    job.BeginDate = data.BeginDate.Value.ToLocalTime().Date;
                }
                else
                {
                    job.BeginDate = data.BeginDate.Value.Date;
                }
                
            }
            if (data.CustomerInfo != null)
            {
                job.CustomerInfo = new JobCustomerInfo()
                {
                    EstimatedClosingDate = data.CustomerInfo.EstimatedClosingDate,
                    Info = data.CustomerInfo.Info,
                    InvoiceAmount = data.CustomerInfo.InvoiceAmount,
                    OrderAmount = data.CustomerInfo.OrderAmount,
                    MinutesQuotation = data.CustomerInfo.MinutesQuotation,
                    OrderRef = data.CustomerInfo.OrderRef,
                    Quotation = data.CustomerInfo.Quotation,
                    QuotationRef = data.CustomerInfo.QuotationRef,
                };
            }


            return job;
        }


        private Job FillFromAddRequest(Job job, DAAddJobRequest data)
        {

            FillFromRequest(job, data.Job);
            if (data.ParentId.HasValue)
            {
                job.Parent = assistantDBContext.GetJob(data.ParentId.Value);
                if (job.Customer == null) job.Customer = job.Parent.Customer;
            }

            return job;
        }

        [HttpPut("{id}")]
        public ActionResult<DWJob> SaveJob(int id, DAJobRequest job)
        {

            try
            {
                var dbJob = assistantDBContext.GetJob(id);
                if (dbJob != null)
                {
                    if (ValidateBase(job, dbJob?.Parent?.JobId))
                    {
                        var dwJob = this.dwObjectFactory.GetDWJob(FillFromRequest(dbJob, job));
                        var validation = new DAModelValidator(Store).Validate(dwJob);
                        if (validation.IsValid)
                        {
                            var result = Store.Store(dwJob);
                            //Tolgo dalla cache, dovrà essere ricaricata
                            memoryCache?.Remove(kJobsCacheKey);
                            if (result.Stored)
                            {

                                if (job.Fields != null)
                                {
                                    foreach (var field in job.Fields)
                                    {
                                        JobExtraFieldValue currentFieldValue = dbJob.Fields.FirstOrDefault(f => f.Id == field.JobFieldId);
                                        if (currentFieldValue == null) currentFieldValue = dbJob.Fields.FirstOrDefault(f => f.ExtraField.Id == field.FieldId);
                                        if (currentFieldValue == null)
                                        {
                                            currentFieldValue = new JobExtraFieldValue();
                                            currentFieldValue.ExtraField = assistantDBContext.ExtraFields.FirstOrDefault(f => f.Id == field.FieldId);

                                            if (currentFieldValue.ExtraField != null)
                                            {
                                                dbJob.Fields.Add(currentFieldValue);
                                            }
                                        }
                                        SetExtraFieldValues(currentFieldValue, field);
                                    }
                                    assistantDBContext.Jobs.Update(dbJob);
                                    assistantDBContext.SaveChanges();
                                }
                                return this.GetJob(dbJob.JobId);
                            }
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

        private void SetExtraFieldValues(JobExtraFieldValue extraFieldValue, DAJobFieldValue field)
        {
            if (extraFieldValue == null || extraFieldValue.ExtraField == null) return;
            switch (extraFieldValue.ExtraField.Type)
            {
                case ExtraFieldType.Bool:
                    var boolData = field.NumberValue == 1;
                    extraFieldValue.NumberValue = field.NumberValue;
                    extraFieldValue.TextValue = boolData ? "SI" : "NO";
                    break;
                case ExtraFieldType.Date:
                    extraFieldValue.DateTimeValue = field.DateTimeValue;
                    extraFieldValue.TextValue = extraFieldValue.DateTimeValue.HasValue ? extraFieldValue.DateTimeValue.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    extraFieldValue.NumberValue = extraFieldValue.DateTimeValue.HasValue ? extraFieldValue.DateTimeValue.Value.Ticks : 0;
                    break;
                case ExtraFieldType.Numeric:
                    extraFieldValue.NumberValue = field.NumberValue;
                    extraFieldValue.TextValue = extraFieldValue.NumberValue.ToString();
                    break;
                default:
                    extraFieldValue.TextValue = field.TextValue;
                    break;

            }
        }

        [HttpPost()]
        public ActionResult<DWJob> InsertJob(DAAddJobRequest job)
        {

            try
            {
                if (Validate(job))
                {

                    var dwJob = this.dwObjectFactory.GetDWJob(FillFromAddRequest(new Job(), job), false);
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

        [HttpGet("{jobId}/subjobs")]
        public ActionResult<DWJob[]> GetSubJobs(int jobId)
        {
            try
            {
                var subJobs = assistantDBContext.Jobs
                    .Include(sj => sj.Parent).ThenInclude(j => j.Customer)
                    .Include(sj => sj.Customer)
                    .Where(sj => sj.Parent.JobId == jobId)
                    .Select(sj => dwObjectFactory.GetDWJob(sj, false));
                return subJobs.ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpGet("{jobId}/fields")]
        public ActionResult<DWJobExtraFieldValue[]> GetJobExtraFields(int jobId)
        {
            try
            {
                var fields = assistantDBContext.JobExtraFields
                    .Include(f => f.ExtraField).ThenInclude(ef => ef.ValuesDomain)
                    .Where(f => f.Job.JobId == jobId)
                    .OrderBy(f => f.ExtraField.Order)
                    .Select(sj => dwObjectFactory.GetDWJobField(sj));
                return fields.ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        private IQueryable<DayWorkLog> MakeDayWorkLogsQuery()
        {
            return assistantDBContext.DayWorkLogs.Include(w => w.Job).ThenInclude(j => j.Parent)
                .Include(w => w.User)
                .Include(w => w.WorkType);
        }

        private IQueryable<DayWorkLog> MakeDayWorkLogsQueryWithDateRange(DateTime startDate, DateTime endDate, int jobId)
        {
            return MakeDayWorkLogsQuery()
                .Where(d => d.Date.Date >= startDate && d.Date.Date <= endDate && d.Job.JobId == jobId).OrderBy(d => d.Date);
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

using Delineat.Assistant.API.Helpers;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Delineat.Assistant.Core.ObjectFactories;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubJobsController : BaseController
    {
        private readonly DADWObjectFactory dwObjectFactory;

        public DAAssistantDBContext assistantDBContext { get; }

        public SubJobsController(DAAssistantDBContext assistantDBContext, ILogger<DayWorkTypesController> logger) : base(logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
        }

        private bool Validate(SubJob subJob)
        {
            if (subJob == null) ModelState.AddModelError(nameof(subJob), "Sotto commessa non valorizzata");
            if (subJob.Job == null)
            {
                ModelState.AddModelError(nameof(subJob.Job), "Commessa non valorizzata");
            }
            else
            {
                //Verifico se esiste un sotto commessa con lo stesso codice per la commessa
                var sameCodeSubJob = assistantDBContext.SubJobs.FirstOrDefault(sj => sj.SubJobId != subJob.SubJobId && sj.Code == subJob.Code && sj.Job.JobId == subJob.Job.JobId);
                if (sameCodeSubJob != null)
                {
                    ModelState.AddModelError(nameof(subJob.Code), $"Esiste già una sottocommessa con il codice '{subJob.Code}'");
                }
            }
            return ModelState.IsValid;
        }

        private bool Validate(DASubJobRequest data, int? id = null)
        {
            if (data == null) ModelState.AddModelError(nameof(data), "Sotto commessa non valorizzato");
            if (string.IsNullOrWhiteSpace(data.Code))
            {
                ModelState.AddModelError(nameof(data.Code), "Codice non valorizzato");
            }
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                ModelState.AddModelError(nameof(data.Description), "Descrizione non valorizzata");
            }

            return ModelState.IsValid;
        }

        private SubJob FillFromRequest(SubJob subJob, DASubJobRequest data)
        {

            subJob.Code = data.Code;
            subJob.Description = data.Description;
            subJob.Customer = assistantDBContext.GetCustomer(data.CustomerId);
            if (subJob.Customer == null) subJob.Customer = subJob.Job?.Customer;
            return subJob;
        }

        

        [HttpGet("{id}")]
        public ActionResult<DWSubJob> GetSubJob(int id)
        {
            try
            {
                var subJob = assistantDBContext.SubJobs
                        .Include(sj => sj.Job)
                        .Include(sj => sj.Customer)
                        .Where(sj => sj.SubJobId == id)
                        .Select(sj => dwObjectFactory.GetDWSubJob(sj))
                        .FirstOrDefault();

                if (subJob != null)
                {
                    return subJob;
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
        public ActionResult<DWSubJob> UpdateSubJob(int id, DASubJobRequest data)
        {
            try
            {
                if (Validate(data, id))
                {

                    var subJob = assistantDBContext.SubJobs
                        .Include(sj => sj.Job)
                        .Include(sj => sj.Customer)
                        .FirstOrDefault(sj => sj.SubJobId == id);

                    if (subJob != null)
                    {
                        if (Validate(FillFromRequest(subJob, data)))
                        {
                            subJob.UpdateDate = DateTime.Now;
                            assistantDBContext.SubJobs.Update(subJob);
                            assistantDBContext.SaveChanges();

                            return dwObjectFactory.GetDWSubJob(subJob);
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
        public ActionResult<DWSubJob> InsertSubJob(int jobId, DASubJobRequest data)
        {
            try
            {
                if (Validate(data))
                {
                    //Recupero il job
                    var job = assistantDBContext.GetJob(jobId);
                    if (job == null) return NotFound();

                    var subJob = new SubJob();
                    subJob.Job = job;
                    if (Validate(FillFromRequest(subJob, data)))
                    {

                        subJob.InsertDate = DateTime.Now;
                        assistantDBContext.SubJobs.Add(subJob);
                        assistantDBContext.SaveChanges();

                        return CreatedAtAction(nameof(GetSubJob), new { id = subJob.SubJobId }, dwObjectFactory.GetDWSubJob(subJob));
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


        [HttpDelete("{id}")]
        public ActionResult DeleteSubJob(int id)
        {
            try
            {
                var subJob = assistantDBContext.SubJobs.Include(sj => sj.Items).FirstOrDefault(sj => sj.SubJobId == id);
                if (subJob != null)
                {
                    // Verifico se il cliente ha delle commesse collegate
                    // Se si lo annullo logicamente altrimenti lo rimuovo
                    if (subJob.Items != null && subJob.Items.Count() > 0)
                    {
                        subJob.DeleteDate = DateTime.Now;
                        assistantDBContext.SubJobs.Update(subJob);
                    }
                    else
                    {                       
                        assistantDBContext.SubJobs.Remove(subJob);
                    }

                    assistantDBContext.SaveChanges();
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
    }
}

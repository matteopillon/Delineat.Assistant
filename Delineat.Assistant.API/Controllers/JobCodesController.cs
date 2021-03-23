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
    public class JobCodesController : BaseController
    {
        private readonly DADWObjectFactory dwObjectFactory;

        public DAAssistantDBContext assistantDBContext { get; }

        public JobCodesController(DAAssistantDBContext assistantDBContext, ILogger<DayWorkTypesController> logger) : base(logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
        }

        private bool Validate(JobCode jobCode)
        {
            if (jobCode == null) ModelState.AddModelError(nameof(jobCode), "Codice extra non valorizzato");
            if (string.IsNullOrWhiteSpace(jobCode.Code))
            {
                ModelState.AddModelError(nameof(jobCode.Code), "Codice non valorizzato");
            }
            if (jobCode.Job == null)
            {
                ModelState.AddModelError(nameof(jobCode.Job), "Commessa non valorizzata");
            }
            else
            {

                if (jobCode.Job.Codes.FirstOrDefault(jc => jc.Code.ToLower() == (jobCode.Code ?? string.Empty).ToLower()
                                                        && jc.CodeId != jobCode.CodeId) != null)
                {
                    ModelState.AddModelError(nameof(jobCode.Code), "Codice già inserito");
                }

            }

            return ModelState.IsValid;
        }

        private bool Validate(DAJobCodeRequest data, int? id = null)
        {
            if (data == null) ModelState.AddModelError(nameof(data), "Codice extra non valorizzato");
            if (string.IsNullOrWhiteSpace(data.Code))
            {
                ModelState.AddModelError(nameof(data.Code), "Codice non valorizzato");
            }

            return ModelState.IsValid;
        }

        private JobCode FillFromRequest(JobCode jobCode, DAJobCodeRequest data)
        {

            jobCode.Code = data.Code;
            jobCode.Note = data.Note;

            return jobCode;
        }



        [HttpGet("{id}")]
        public ActionResult<DWJobCode> GetJobCode(int id)
        {
            try
            {
                var jobCode = assistantDBContext.JobCodes
                        .Include(jc => jc.Job)
                        .ThenInclude(j => j.Codes)
                        .Select(jc => dwObjectFactory.GetDWJobCode(jc))
                        .FirstOrDefault();

                if (jobCode != null)
                {
                    return jobCode;
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
        public ActionResult<DWJobCode> UpdateJobCode(int id, DAJobCodeRequest data)
        {
            try
            {
                if (Validate(data, id))
                {

                    var jobCode = assistantDBContext.JobCodes
                        .Include(jc => jc.Job)
                        .FirstOrDefault(jc => jc.CodeId == id);

                    if (jobCode != null)
                    {
                        if (Validate(FillFromRequest(jobCode, data)))
                        {
                            assistantDBContext.JobCodes.Update(jobCode);
                            assistantDBContext.SaveChanges();

                            return dwObjectFactory.GetDWJobCode(jobCode);
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
        public ActionResult<DWJobCode> InsertSubJob(int jobId, DAJobCodeRequest data)
        {
            try
            {
                if (Validate(data))
                {
                    //Recupero il job
                    var job = assistantDBContext.GetJob(jobId);
                    if (job == null) return NotFound();

                    var jobCode = new JobCode();
                    jobCode.Job = job;
                    if (Validate(FillFromRequest(jobCode, data)))
                    {


                        assistantDBContext.JobCodes.Add(jobCode);
                        assistantDBContext.SaveChanges();

                        return CreatedAtAction(nameof(GetJobCode), new { id = jobCode.CodeId }, dwObjectFactory.GetDWJobCode(jobCode));
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
                var jobCode = assistantDBContext.JobCodes.FirstOrDefault(jc => jc.CodeId == id);
                if (jobCode != null)
                {
                    assistantDBContext.JobCodes.Remove(jobCode);

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

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
using System.Linq;
namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DayWorkLogsController : BaseController
    {
        private readonly DADWObjectFactory dwObjectFactory;
        private readonly DADataObjectFactory dataObjectFactory;
        private readonly DAAssistantDBContext assistantDBContext;

        public DayWorkLogsController(DAAssistantDBContext assistantDBContext, ILogger<DayWorkLogsController> logger) : base(logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.dataObjectFactory = new DADataObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
        }


        private IQueryable<DayWorkLog> MakeDayWorkLogsQuery()
        {
            return assistantDBContext.DayWorkLogs.Include(w => w.Job)
                .Include(w => w.User)
                .Include(w => w.WorkType)
                .Include(w => w.SubJob);
        }

        private IQueryable<DayWorkLog> MakeDayWorkLogsQueryWithDateRange(DateTime startDate, DateTime endDate)
        {
            return MakeDayWorkLogsQuery()
                .Where(d => d.Date.Date >= startDate && d.Date.Date <= endDate);
        }

        [HttpGet()]
        public ActionResult<DWDayWorkLog[]> GetWorkLogs(DateTime startDate, DateTime endDate, int? userId)
        {
            try
            {
                var query = MakeDayWorkLogsQueryWithDateRange(startDate, endDate);
                if (userId.HasValue)
                {
                    query.Where(d => d.User.UserId == userId);
                }

                return query.Select(d => dwObjectFactory.GetDWDayWorkLog(d)).ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<DWDayWorkLog> GetWorkLog(int id)
        {
            try
            {
                var log = MakeDayWorkLogsQuery()
                    .Where(w => w.DayWorkLogId == id)
                    .Select(d => dwObjectFactory.GetDWDayWorkLog(d)).FirstOrDefault();
                if (log != null)
                {
                    return log;
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



        private DayWorkLog FillFromRequest(DayWorkLog log, DayWorkLogRequest data)
        {
            if (data.Date.Kind == DateTimeKind.Utc)
            {
                log.Date = data.Date.ToLocalTime().Date;
            }
            else
            {
                log.Date = data.Date;
            }

            log.Job = assistantDBContext.GetJob(data.JobId);
            log.Minutes = data.Minutes;
            log.SubJob = data.SubJobId > 0 ? assistantDBContext.GetSubJob(data.SubJobId) : null;
            log.User = assistantDBContext.GetUser(data.UserId);
            log.WorkType = assistantDBContext.GetDayWorkType(data.DayWorkTypeId);
            log.Note = data.Note;

            return log;
        }

        private bool Validate(DayWorkLog log)
        {
            if (log == null) ModelState.AddModelError(nameof(log), "DayWorkLog non trovato");
            if (log.Job == null) ModelState.AddModelError(nameof(log.Job), "Commessa non trovata");
            if (log.User == null) ModelState.AddModelError(nameof(log.User), "Utente non trovato");
            if (log.Minutes <= 0) ModelState.AddModelError(nameof(log.Minutes), "Le ore devono essere valorizzate");
            if (log.WorkType == null) ModelState.AddModelError(nameof(log.WorkType), "Il tipo di registrazione deve essere valorizzato");
            if (log.SubJob?.Job != null && log.Job != null && log.Job.JobId != log.SubJob.Job.JobId) ModelState.AddModelError(nameof(log.SubJob), $"La sotto commessa {log.SubJob.Description} non appartiene alla commessa {log.Job.Description}");
            return ModelState.IsValid;
        }

        private bool Validate(DayWorkLogRequest data)
        {
            if (data == null) ModelState.AddModelError(nameof(data), "WorkLog non valorizzato");
            if (data.JobId == 0) ModelState.AddModelError(nameof(DayWorkLog.Job), "Commessa non valorizzata");
            if (data.UserId == 0) ModelState.AddModelError(nameof(DayWorkLog.User), "Utente non valorizzato");
            if (data.Minutes <= 0) ModelState.AddModelError(nameof(DayWorkLog.Minutes), "Le ore devono essere valorizzate");
            if (data.DayWorkTypeId == 0) ModelState.AddModelError(nameof(DayWorkLog.WorkType), "Il tipo di registrazione deve essere valorizzato");

            return ModelState.IsValid;
        }

        [HttpPut("{id}")]
        public ActionResult<DWDayWorkLog> UpdateWorkLog(int id, DayWorkLogRequest data)
        {
            try
            {
                if (Validate(data))
                {

                    var log = assistantDBContext.DayWorkLogs.FirstOrDefault(l => l.DayWorkLogId == id);

                    if (log != null)
                    {
                        if (Validate(FillFromRequest(log, data)))
                        {

                            assistantDBContext.DayWorkLogs.Update(log);
                            assistantDBContext.SaveChanges();

                            return dwObjectFactory.GetDWDayWorkLog(log);
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
        public ActionResult<DWDayWorkLog> InsertWorkLog(DayWorkLogRequest data)
        {
            try
            {
                if (Validate(data))
                {

                    var log = new DayWorkLog();
                    if (Validate(FillFromRequest(log, data)))
                    {

                        assistantDBContext.DayWorkLogs.Add(log);
                        assistantDBContext.SaveChanges();

                        return CreatedAtAction(nameof(GetWorkLog), new { id = log.DayWorkLogId }, dwObjectFactory.GetDWDayWorkLog(log));
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
        public ActionResult DeleteWorkLog(int id)
        {
            try
            {
                var worklog = assistantDBContext.DayWorkLogs.FirstOrDefault(w => w.DayWorkLogId == id);
                if (worklog != null)
                {
                    assistantDBContext.DayWorkLogs.Remove(worklog);
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

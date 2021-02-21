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

        public DayWorkLogsController(DAAssistantDBContext assistantDBContext, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.dataObjectFactory = new DADataObjectFactory();
            this.assistantDBContext = assistantDBContext;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<DayWorkLogsController>();
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
                .Where(d => d.Date >= startDate && d.Date <= endDate);
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
                return MakeDayWorkLogsQuery()
                    .Where(w => w.DayWorkLogId == id)
                    .Select(d => dwObjectFactory.GetDWDayWorkLog(d)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpPut()]
        public ActionResult<DWDayWorkLog> UpInsertWorkLog(DWDayWorkLog workLog)
        {
            try
            {
                if (workLog == null) return BadRequest("WorkLog non valorizzato");
                if (workLog.Job == null) return BadRequest("Commessa non valorizzata");
                if (workLog.User == null) return BadRequest("Utente non valorizzato");
                if (workLog.Minutes <= 0) return BadRequest("Le ore devono essere valorizzate");

                var saveWorkLog = dataObjectFactory.GetDBDayWorkLog(workLog);
                if (saveWorkLog.Job != null) assistantDBContext.Attach(saveWorkLog.Job);
                if (saveWorkLog.User != null) assistantDBContext.Attach(saveWorkLog.User);
                if (saveWorkLog.WorkType != null) assistantDBContext.Attach(saveWorkLog.WorkType);
                if (saveWorkLog.SubJob != null) assistantDBContext.Attach(saveWorkLog.SubJob);

                if (workLog.DayWorkLogId > 0)
                {
                    var worklog = assistantDBContext.DayWorkLogs.FirstOrDefault(w => w.DayWorkLogId == workLog.DayWorkLogId);
                    if (worklog != null)
                    {
                        assistantDBContext.DayWorkLogs.Update(saveWorkLog);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    assistantDBContext.DayWorkLogs.Add(saveWorkLog);
                }

                assistantDBContext.SaveChanges();

                return dwObjectFactory.GetDWDayWorkLog(saveWorkLog);
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

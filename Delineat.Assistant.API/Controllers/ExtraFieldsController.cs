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
    public class ExtraFieldsController : BaseController
    {
        private readonly DADWObjectFactory dwObjectFactory;
        private readonly DADataObjectFactory dataObjectFactory;
        private readonly DAAssistantDBContext assistantDBContext;

        public ExtraFieldsController(DAAssistantDBContext assistantDBContext, ILogger<DayWorkLogsController> logger) : base(logger)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.dataObjectFactory = new DADataObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
        }

        [HttpGet()]
        public ActionResult<DWExtraField[]> GetExtraFields(DateTime startDate, DateTime endDate, int? userId)
        {
            try
            {
                return assistantDBContext.ExtraFields
                    .Include(ef => ef.ValuesDomain)
                    .OrderBy(ef => ef.Order)
                    .Select(ef => dwObjectFactory.GetDWExtraField(ef)).ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<DWExtraField> GetExtraField(int id)
        {
            try
            {
                var field = assistantDBContext.ExtraFields.Include(ef => ef.ValuesDomain)
                    .Where(ef=>ef.Id == id)
                    .Select(ef => dwObjectFactory.GetDWExtraField(ef))
                    .FirstOrDefault();
                if (field != null)
                {
                    return field;
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

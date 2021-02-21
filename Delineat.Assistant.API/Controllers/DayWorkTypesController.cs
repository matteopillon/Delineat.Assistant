using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.ObjectFactories;
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
    public class DayWorkTypesController : BaseController
    {
        private readonly DADWObjectFactory dwObjectFactory;
        private readonly DAAssistantDBContext assistantDBContext;

        public DayWorkTypesController(DAAssistantDBContext assistantDBContext, ILoggerFactory loggerFactory) : base( loggerFactory)
        {
            this.dwObjectFactory = new DADWObjectFactory(assistantDBContext);
            this.assistantDBContext = assistantDBContext;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<DayWorkTypesController>();
        }



        [HttpGet()]
        public ActionResult<DWDayWorkType[]> GetDayWorkTypes()
        {

            try
            {
                return assistantDBContext.DayWorkTypes.Select(d => dwObjectFactory.GetDWDayWorkType(d)).ToArray();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Delineat.Assistant.API.Controllers
{
    public class BaseController:ControllerBase
    {
        protected readonly ILoggerFactory loggerFactory;
        protected readonly ILogger logger;
        public BaseController(ILoggerFactory loggerFactory)
        {         
            this.loggerFactory = loggerFactory;
            this.logger = MakeLogger(loggerFactory);
        }

        protected virtual ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<BaseController>();
        }

        protected ObjectResult Problem(Exception ex)
        {
            logger?.LogError(ex, ex.Message);
            return Problem(ex.Message);
        }

       
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Delineat.Assistant.API.Controllers
{
    public class BaseController:ControllerBase
    {
        protected readonly ILogger logger;
        public BaseController(ILogger logger)
        {         
            this.logger = logger;        
        }

        protected ObjectResult Problem(Exception ex)
        {
            logger?.LogError(ex, ex.Message);
            return Problem(ex.Message);
        }

       
    }
}

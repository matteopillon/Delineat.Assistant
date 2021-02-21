using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Models.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : BaseController
    {
        private readonly IOptions<DAClientConfiguration> clientConfiguration;

        public ConfigController(IOptions<DAClientConfiguration> clientConfiguration, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            this.clientConfiguration = clientConfiguration;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<ConfigController>();
        }

        [HttpGet]
        public ActionResult<DAConfigResult> Get()
        {
            var response = new DAConfigResult();

            if (clientConfiguration.Value != null)
            {
                response.Categories.AddRange(clientConfiguration.Value.Categories ?? new List<string>());
                response.Emails.AddRange(clientConfiguration.Value.Emails ?? new List<string>());
            }
            return response;
        }
    }
}

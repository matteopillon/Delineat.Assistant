using Delineat.Assistant.Core.Users;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IDAUsersStore usersStore;

        public UsersController(ILoggerFactory loggerFactory, IDAUsersStore usersStore) : base(loggerFactory)
        {
            this.usersStore = usersStore;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<UsersController>();
        }

        [HttpGet]
        public ActionResult<DWUser[]> GetUsers()
        {
            try
            {
                return Ok(usersStore.GetUsers());

            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }
    }
}

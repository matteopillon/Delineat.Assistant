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

        public UsersController(ILogger<UsersController> logger, IDAUsersStore usersStore) : base(logger)
        {
            this.usersStore = usersStore;
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

using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.ObjectFactories;
using Delineat.Assistant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Users
{
    public class DADBUsersStore : IDAUsersStore
    {
        private readonly ILogger<DADBUsersStore> logger;
        private readonly DAAssistantDBContext dataContext;
        private readonly DADataObjectFactory dataObjectFactory;
        private readonly DADWObjectFactory dwObjectFactory;

        public DADBUsersStore(ILogger<DADBUsersStore> logger, DAAssistantDBContext dataContext)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.dataObjectFactory = new DADataObjectFactory(dataContext);
            this.dwObjectFactory = new DADWObjectFactory(dataContext);
        }

        public bool DeleteUser(int id)
        {
            var user = dataContext.Users.FirstOrDefault(u => u.UserId == id);
            if(user != null)
            {
                dataContext.Users.Remove(user);
                dataContext.SaveChanges();
            }

            return false;
        }

        public DWUser GetUser(int id)
        {
            return dwObjectFactory.GetDWUser(dataContext.Users.Include(u => u.Roles).ThenInclude(r => r.Permissions).FirstOrDefault(u => u.UserId == id));
        }

        public IEnumerable<DWUser> GetUsers()
        {
            return dataContext.Users.Include(u=>u.Roles).ThenInclude(r=>r.Permissions).Select(u => dwObjectFactory.GetDWUser(u));
        }

        public bool UpInsertUser(DWUser user)
        {

           
           

            return false;
        }
    }
}

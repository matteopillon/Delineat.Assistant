using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Helpers
{
    public static class DBContextHelper
    {
        public static User GetUser(this DAAssistantDBContext dbContext, int id)
        {
            return dbContext.Users.FirstOrDefault(j => j.UserId == id);
        }
        public static Job GetJob(this DAAssistantDBContext dbContext, int id)
        {
            return dbContext.Jobs.Include(j => j.Customer).FirstOrDefault(j => j.JobId == id);
        }

        public static SubJob GetSubJob(this DAAssistantDBContext dbContext, int id)
        {
            return dbContext.SubJobs.FirstOrDefault(j => j.SubJobId == id);
        }

        public static DayWorkType GetDayWorkType(this DAAssistantDBContext dbContext, int id)
        {
            return dbContext.DayWorkTypes.FirstOrDefault(j => j.DayWorkTypeId == id);
        }

        public static Customer GetCustomer(this DAAssistantDBContext dbContext, int id)
        {
            return dbContext.Customers.FirstOrDefault(j => j.CustomerId == id);
        }

    }
}

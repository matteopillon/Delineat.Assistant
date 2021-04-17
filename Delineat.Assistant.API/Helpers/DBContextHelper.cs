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
            return dbContext.Jobs.Include(j => j.Customer).Include(j=>j.Codes).Include(j=>j.Fields).ThenInclude(f=>f.ExtraField).FirstOrDefault(j => j.JobId == id);
        }

        public static Item GetItem(this DAAssistantDBContext dbContext, int id)
        {
            return dbContext.Items.FirstOrDefault(i => i.ItemId == id);
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

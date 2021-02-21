using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Models
{
   
    public class DAStoreItemRequest
    {
        public DWItem Item { get; set; }
        public Guid[] LoadingSessions { get; set; }
    }
}

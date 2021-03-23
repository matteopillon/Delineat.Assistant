using Delineat.Assistan.Exports;
using Delineat.Assistant.API.Helpers;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.Core.Data;
using Delineat.Assistant.Core.Data.Models;
using Delineat.Assistant.Core.ObjectFactories;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportsController : BaseController
    {
       private readonly DAAssistantDBContext assistantDBContext;

        public ExportsController(DAAssistantDBContext assistantDBContext, ILogger<ExportsController> logger) : base(logger)
        {
             this.assistantDBContext = assistantDBContext;
        }


       

        [HttpGet("montlyhours/{year}/{month}")]
        public FileResult GetWorkLogs(int year,int month)
        {
            try
            {
                // Creo un file temporaneo excel
                var path = Path.ChangeExtension( Path.GetTempFileName(),"xlsx");
                var export = new MothlyHoursExport(assistantDBContext);
                if (export.ExportToExcel(path, month, year))
                {
                    var bytes = System.IO.File.ReadAllBytes(path);
                    System.IO.File.Delete(path);

                    var cultureInfo = new CultureInfo("it-IT");

                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",$"{new DateTime(year,month,1).ToString("MMMM",cultureInfo).Capitalize()}_{year}.xlsx");
                }
                else
                {
                    throw new ApplicationException("Errore in fase di generazione export paghe mensile");
                }

            }
            catch (Exception ex)
            {
                logger?.LogError("Errore in fase di generazione export paghe mensile",ex);
                throw;
            }
        }       
    }
}

using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Stores.Exceptions;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentVersionsController : StoreBaseController
    {
        private readonly DAServerConfiguration serverConfiguration = new DAServerConfiguration();

        public DocumentVersionsController(Microsoft.Extensions.Options.IOptions<DAStoresConfiguration> storesConfiguration,
            IOptions<DAServerConfiguration> serverConfigurationOptions, ILoggerFactory loggerFactory) : base(storesConfiguration, loggerFactory)
        {
            if (serverConfigurationOptions != null)
                serverConfiguration = serverConfigurationOptions.Value;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<DocumentVersionsController>();
        }

        [HttpGet("{id}")]
        public IActionResult GetDocumentVersion(int id)
        {
            try
            {
                var stream = new MemoryStream();
                var stores = GetStores();

                foreach (var store in stores)
                {
                    byte[] documentVersion = store.GetDocumentVersionData(id);
                    stream = new MemoryStream(documentVersion);
                    return this.File(stream, "application/octet-stream");
                }


            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
            return NotFound();
        }

        [HttpGet("{id}/path")]
        public ActionResult<string> GetDocumentVersionPath(int id)
        {
            try
            {
                var stores = GetStores();

                foreach (var store in stores)
                {
                    var documentVersionPath = store.GetDocumentVersionPath(id);
                    return documentVersionPath;
                }


            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
            return NotFound();
        }


        [HttpPut("{id}/status")]
        public ActionResult<DWDocumentVersion> SetDocumentVersionStatus(int id, [FromBody] JObject request)
        {
            var status = request["status"].ToObject<DocumentVersionStatus>();

            return SetDocumentVersionValues(id, (docVersion) =>
            {
                docVersion.Status = status;
                docVersion.StatusSince = DateTime.Now;
            });
        }

        [HttpPut("{id}/InEvidence")]
        public ActionResult<DWDocumentVersion> SetDocumentVersionInEvidence(int id, [FromBody] JObject request)
        {
            var inEvidence = request["inEvidence"].ToObject<bool>();

            return SetDocumentVersionValues(id, (docVersion) =>
            {
                docVersion.InEvidence = inEvidence;
            });
        }

        [HttpPut("{id}/waitingForReply")]
        public ActionResult<DWDocumentVersion> SetDocumentVersionWaitingForReply(int id, [FromBody] JObject request)
        {
            var dateTime = request["date"].ToObject<DateTime?>();
            return SetDocumentVersionValues(id, (docVersion) =>
            {
                docVersion.WaitingForReply = dateTime;
            });
        }

        [HttpPut("{id}/reply")]
        public ActionResult<DWDocumentVersion> SetDocumentVersionReply(int id, [FromBody] JObject request)
        {
            var dateTime = request["date"].ToObject<DateTime?>();

            return SetDocumentVersionValues(id, (docVersion) =>
            {
                docVersion.Reply = dateTime;
            });
        }



        private ActionResult<DWDocumentVersion> SetDocumentVersionValues(int id, Action<DWDocumentVersion> setValuesAction)
        {

            try
            {

                var stores = GetStores();
                foreach (var store in stores)
                {
                    var docVersion = store.GetDocumentVersion(id);

                    if (docVersion != null)
                    {
                        setValuesAction(docVersion);
                        var result = store.UpdateDocumentVersion(docVersion);
                        if (result.Stored)
                            return result.Data;
                        else
                            return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessages);                       
                    }                    
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
           
        }

    }
}

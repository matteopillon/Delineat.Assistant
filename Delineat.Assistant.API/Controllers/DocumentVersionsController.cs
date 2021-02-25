using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Models;
using Delineat.Assistant.API.Validators;
using Delineat.Assistant.Core.Interfaces;
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

        public DocumentVersionsController(IDAStore store,
            IOptions<DAServerConfiguration> serverConfigurationOptions, ILogger<DocumentVersionsController> logger) : base(store, logger)
        {
            if (serverConfigurationOptions != null)
                serverConfiguration = serverConfigurationOptions.Value;
        }

        [HttpGet("{id}")]
        public IActionResult GetDocumentVersion(int id)
        {
            try
            {
                var stream = new MemoryStream();

                byte[] documentVersion = Store.GetDocumentVersionData(id);
                if (documentVersion != null)
                {
                    stream = new MemoryStream(documentVersion);
                    return this.File(stream, "application/octet-stream");
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }

        [HttpGet("{id}/path")]
        public ActionResult<string> GetDocumentVersionPath(int id)
        {
            try
            {

                var documentVersionPath = Store.GetDocumentVersionPath(id);
                if (!string.IsNullOrWhiteSpace(documentVersionPath))
                {
                    return documentVersionPath;
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
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
                var docVersion = Store.GetDocumentVersion(id);

                if (docVersion != null)
                {
                    setValuesAction(docVersion);
                    var result = Store.UpdateDocumentVersion(docVersion);
                    if (result.Stored)
                        return result.Data;
                    else
                        return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessages);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }

        }

    }
}

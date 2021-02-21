using Delineat.Assistant.API.Configuration;
using Delineat.Assistant.API.Managers;
using Delineat.Assistant.API.Models.Results;
using Delineat.Assistant.Core.Helpers;
using Delineat.Assistant.Core.Stores.Configuration;
using Delineat.Assistant.Core.Tips;
using Delineat.Assistant.Core.Tips.Email;
using Delineat.Assistant.Core.Tips.Email.EML;
using Delineat.Assistant.Core.Tips.Email.Outlook;
using Delineat.Assistant.Core.Tips.Interfaces;
using Delineat.Assistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : StoreBaseController
    {
        private readonly DAServerConfiguration serverConfiguration = new DAServerConfiguration();

        public FilesController(IOptions<DAStoresConfiguration> storesConfiguration, IOptions<DAServerConfiguration> serverConfigurationOptions, ILoggerFactory loggerFactory) : base(storesConfiguration, loggerFactory)
        {
            if (serverConfigurationOptions != null)
                serverConfiguration = serverConfigurationOptions.Value;
        }

        protected override ILogger MakeLogger(ILoggerFactory loggerFactory)
        {
            return loggerFactory.CreateLogger<FilesController>();
        }

        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public  DAUploadResult Upload(IFormFile file)
        {
            var result = new DAUploadResult();


            var fileName = Path.GetFileName(file.FileName);
           
            using (var fs = file.OpenReadStream())
            {
                DALoadingSessionManager sessionManager = new DALoadingSessionManager(serverConfiguration.SessionsPath, logger);
                var loadingSession = sessionManager.CreateSession();
                var sessionFilePath = sessionManager.AddFileToSession(loadingSession, fileName, fs, result.NeedPasswordFiles);

                result.LoadingSessionId = loadingSession.Id;
                result.FileName = fileName;

                DASessionTipsAttachmentsStoreManager attachmentsStore = new DASessionTipsAttachmentsStoreManager(loadingSession);
                //Caricamento dei tips                       
                IDWTipsFiller tipsFiller = CreateTipsFiller();
                result.Tips = tipsFiller.Fill(sessionFilePath, attachmentsStore);
            }

            return result;
        }

        [HttpPost("unzip")]
        public ActionResult<DAUploadResult> UnZip(DWFilePassword item)
        {
            try
            {

                DALoadingSessionManager sessionManager = new DALoadingSessionManager(serverConfiguration.SessionsPath, logger);
                var loadingSession = sessionManager.GetSessionWithId(item.SessionId);
                var protectedFiles = new List<string>();
                var extractResult = sessionManager.ExtractFileWithPassword(loadingSession, item, protectedFiles);
                var result = new DAUploadResult();
                result.NeedPasswordFiles.AddRange(protectedFiles);
                result.LoadingSessionId = item.SessionId;
                result.FileName = System.IO.Path.GetFileName(item.Path);
                return result;
            }
            catch (Exception ex)
            {
                return Problem(ex);
            }
        }



        private IDWTipsFiller CreateTipsFiller()
        {
            DWMultiItemFiller filler = new DWMultiItemFiller();
            filler.Fillers.Add(new DWEmailFiller(new DWEMLMsgReader()));
            filler.Fillers.Add(new DWEmailFiller(new DWOutlookMsgReader()));
            return filler;
        }

        private static bool IsMultipartContentType(string contentType)
        {
            return
                !string.IsNullOrEmpty(contentType) &&
                contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var element = elements.Where(entry => entry.StartsWith("boundary=")).First();
            var boundary = element.Substring("boundary=".Length);
            // Remove quotes
            if (boundary.Length >= 2 && boundary[0] == '"' &&
                boundary[boundary.Length - 1] == '"')
            {
                boundary = boundary.Substring(1, boundary.Length - 2);
            }
            return boundary;
        }

        private string GetFileName(string contentDisposition)
        {
            return DAFileHelper.GetSafeFilename(contentDisposition
                .Split(';')
                .SingleOrDefault(part => part.Contains("filename"))
                .Split('=')
                .Last()
                .Trim('"'));
        }

    }
}

using Delineat.Assistant.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Interfaces
{
    public interface IDASyncStore
    {
        string Name { get; set; }
        string Description { get; set; }

        void LoadSettings(Dictionary<string, string> settings);

        List<DWJob> GetJobs();
        List<DWItem> GetJobItems(DWJob job);

        bool SyncDeleteJob(DWJob job);
        bool SyncDeleteItem(DWJob job, DWItem item);
        bool SyncDeleteDocument(DWJob job, DWItem item, DWDocument document);

        bool SyncJob(DWJob job);
        bool SyncItem(DWJob job, DWItem item);

        bool AlreadySync(IDASyncStore syncStore);
        bool BeginSync(IDASyncStore store, ILogger syncLogger);

        bool CompleteSync(IDASyncStore syncStore);

        bool TryRollbackSync(IDASyncStore syncStore);
        bool SyncRemoveDocumemtTag(DWJob dWJob, DWItem dWItem, DWDocument dWDocument, DWTag dWTag);
        bool SyncRemoveItemTag(DWJob dWJob, DWItem dWItem, DWTag dWTag);
        bool SyncAddDocumentTag(DWJob dWJob, DWItem dWItem, DWDocument dWDocument, DWTag dWTag, string fileVersionPath);
        bool SyncAddItemTag(DWJob dWJob, DWItem dWItem, DWTag dWTag);

    }
}

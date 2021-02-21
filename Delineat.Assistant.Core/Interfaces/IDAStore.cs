using Delineat.Assistant.Models;
using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Interfaces
{
    public interface IDAStore: IDASyncStore
    {
        DWStoreInfoWithUpdatedData<int> Store(DWItem item, DWJob job);
        DWStoreInfo Store(DWJob job);
        DWStoreInfo Store(DWTopic topic);

        DWStoreInfo DeleteJob(int jobId);

        List<DWTag> GetTags();
        List<DWWorkLogType> GetWorkLogTypes();

        bool AddJob(DWJob job);

        DWJob GetJob(int jobId);
        DWItem GetItem(int itemId);
        List<DWItem> GetJobItems(int jobId);

        bool UpdateItem(DWItem item, DWJob job);

        List<DWNote> GetJobNotes(int jobId);
        DWStoreInfo AddNoteToJob(int jobId, DWNote note);
        DWStoreInfo AddNoteToItem(int itemId, DWNote note);
        DWStoreInfo AddJobTag(int jobId, DWTag tag);
        List<string> GetJobCategories(int jobId);
        List<DWNote> GetUnremindedNotes();
        DWStoreInfo UpdateNote(DWNote note);
        DWScope GetNoteScope(DWNote note);



        bool InitStore();

        List<IDASyncStore> SyncStores { get; set; }
        bool SetDefaultSyncStore(IDASyncStore syncStore);

        byte[] GetDocumentVersionData(int id);
        string GetDocumentVersionPath(int id);

        DWStoreInfo DeleteItem(int itemId);
        DWStoreInfo DeleteDocument(int documentId);
        DWStoreInfo RemoveItemTag(int itemId, int tagId);
        DWStoreInfo RemoveDocumentTag(int documentId, int tagId);
        DWStoreInfoWithUpdatedData<DWTag[]> AddDocumentTags(int documentId, DWTag[] tags);
        DWStoreInfoWithUpdatedData<DWTag[]> AddItemTags(int itemId, DWTag[] tags);
        DWStoreInfoWithUpdatedData<DWWorkLog> UpdateWorkLog(DWWorkLog workLog);
        DWStoreInfoWithUpdatedData<DWWorkLog> AddWorkLogToItem(int itemId, DWWorkLog log);
        DWDocumentVersion GetDocumentVersion(int documentVersionId);
        DWStoreInfoWithUpdatedData<DWDocumentVersion> UpdateDocumentVersion(DWDocumentVersion documentVersion);
        void UpdateSyncSettings(List<IDASyncStore> syncStores);

    }
}

using Delineat.Assistant.Models;

namespace Delineat.Assistant.Core.Stores.Interfaces
{
    public interface IDADelineatJobsFactory
    {
        DWJob CreateJobFromFolderName(string folderName);
        DWItem CreateItemFromFoldeName(string folderName);

        string CreateFolderNameFromJob(DWJob job);
        string CreateItemFolderFromItem(DWItem item);


    }
}

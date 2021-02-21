using Delineat.Assistant.Models;

namespace Delineat.Assistant.Core.Interfaces
{
    public interface IDAJobsFactory
    {
        DWJob CreateJobFromFolderName(string folderName);

        string CreateFolderNameFromJob(DWJob job);
        string CreateItemFolderFromItem(DWItem item);
    }
}

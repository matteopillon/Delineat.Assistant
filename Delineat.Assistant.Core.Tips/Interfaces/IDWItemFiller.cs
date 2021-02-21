using Delineat.Assistant.Models;

namespace Delineat.Assistant.Core.Tips.Interfaces
{
    public interface IDWTipsFiller
    {
        DWTips Fill(object obj, IDWTipsAttachmentsStore attachmentsStore = null);
    }
}

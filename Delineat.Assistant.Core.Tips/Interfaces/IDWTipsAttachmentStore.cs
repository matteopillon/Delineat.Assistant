using System;

namespace Delineat.Assistant.Core.Tips.Interfaces
{
    public interface IDWTipsAttachmentsStore
    {
        bool StoreFile(string fileName, Byte[] data);
    }
}

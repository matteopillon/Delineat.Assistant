using System.Collections.Generic;

namespace Delineat.Assistant.Models
{

    public class DWStoreInfo
    {
        #region Fields

        private List<string> errorMessages = new List<string>();

        #endregion

        #region Properties

        public bool Stored { get; set; }
        public List<string> ErrorMessages
        {
            get
            {
                return errorMessages;
            }
        }
        #endregion



    }


    public class DWStoreInfoWithUpdatedData<T> : DWStoreInfo
    {
        public T Data { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWTips
    {

        #region Fields
        private List<string> jobIds = new List<string>();
        private List<ItemType> itemTypes = new List<ItemType>();
        private List<string> whos = new List<string>();
        private List<string> descriptions = new List<string>();
        private List<string> attachments = new List<string>();
        private List<string> notes = new List<string>();
        private List<string> categories = new List<string>();
        private List<DateTime> dates = new List<DateTime>();
        #endregion

        #region Readonly Properties
        public List<string> Notes
        {
            get
            {
                return notes;
            }
        }

        public List<string> Tags
        {
            get
            {
                return categories;
            }
        }

        public List<string> Descriptions
        {
            get
            {
                return descriptions;
            }
        }


        public List<string> Whos
        {
            get
            {
                return whos;
            }
        }

        public List<ItemType> ItemTypes
        {
            get
            {
                return itemTypes;
            }
        }

        public List<string> JobIds
        {
            get
            {
                return jobIds;
            }
        }

        public List<string> Attachments
        {
            get
            {
                return attachments;
            }
        }

        public List<DateTime> Dates
        {
            get
            {
                return dates;
            }
        }
        #endregion
    }
}

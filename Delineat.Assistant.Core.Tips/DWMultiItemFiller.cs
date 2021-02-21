using Delineat.Assistant.Core.Tips.Extensions;
using Delineat.Assistant.Core.Tips.Interfaces;
using Delineat.Assistant.Models;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Tips
{

    public class DWMultiItemFiller : IDWTipsFiller
    {
        public DWMultiItemFiller()
        {
            this.fillers = new List<IDWTipsFiller>();
        }

        private List<IDWTipsFiller> fillers;

        public List<IDWTipsFiller> Fillers
        {
            get
            {
                return fillers;
            }
        }

        public DWTips Fill(object obj, IDWTipsAttachmentsStore attachmentsStore)
        {
            DWTips tips = new DWTips();
            foreach (var filler in this.fillers)
            {
                var fillerTips = filler.Fill(obj, attachmentsStore);
                tips.Merge(fillerTips);
            }
            return tips;
        }
    }
}

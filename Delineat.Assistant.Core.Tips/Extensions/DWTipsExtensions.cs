
using Delineat.Assistant.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Delineat.Assistant.Core.Tips.Extensions
{
    public static class DWTipsExtensions
    {
        public static bool IsEmail(this string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }
            else
            {
                return false;
            }
        }

        public static DWTips Merge(this DWTips tip, DWTips tipToMerge)
        {
            MergeList<string>(tip.Tags, tipToMerge.Tags);
            MergeList<string>(tip.Whos, tipToMerge.Whos);
            MergeList<string>(tip.Notes, tipToMerge.Notes);
            MergeList<string>(tip.JobIds, tipToMerge.JobIds);
            MergeList<string>(tip.Descriptions, tipToMerge.Descriptions);
            MergeList<string>(tip.Attachments, tipToMerge.Attachments);
            MergeList<ItemType>(tip.ItemTypes, tipToMerge.ItemTypes);
            MergeList<DateTime>(tip.Dates, tipToMerge.Dates);
            return tip;
        }

        private static void MergeList<T>(List<T> list, List<T> listToMerge)
        {
            foreach (var item in listToMerge)
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
        }

    }
}

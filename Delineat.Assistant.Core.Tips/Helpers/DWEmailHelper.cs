using System.Collections.Generic;

namespace Delineat.Assistant.Core.Tips.Helpers
{
    public static class DWEmailHelper
    {
        public static List<string> InternalEmails { get; set; }

        public static bool IsInternalEmailAddress(string email)
        {
            email = (email ?? string.Empty).ToLower();
            if (InternalEmails != null)
            {
                foreach (var i in InternalEmails)
                    if (email.Contains(i.ToLower()))
                    {
                        return true;
                    }
            }
            return false;
        }

        internal static string GetDomainName(string from)
        {
            var domain = GetDomain(from);
            var index = (domain ?? string.Empty).IndexOf(".");
            if (index >= 0 && index < domain.Length)
            {
                return domain.Substring(0, index);
            }
            else
                return string.Empty;
        }

        internal static string GetDomain(string from)
        {
            if (from == null) return string.Empty;
            var index = (from ?? string.Empty).IndexOf("@") + 1;
            if (index >= 0 && index < from.Length)
            {
                return from.Substring(index++);
            }
            else
                return string.Empty;
        }
    }
}

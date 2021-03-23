using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Helpers
{
    public static class DAStringsHelper
    {
        public static string Capitalize(this string text)
        {
            
            switch((text ?? string.Empty).Length)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return text.ToUpper();
                default:
                    return $"{text[0].ToString().ToUpper()}{text.Substring(1)}";
            }
            
        }
    }
}

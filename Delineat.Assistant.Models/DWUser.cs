using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Models
{
    public enum DWUserRole
    {
        User,
        Admin
    }

    public class DWUser
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Nickname { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public DWUserRole Role { get; set; }
    }
}

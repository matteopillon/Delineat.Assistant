using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Models
{
    public enum DWUserRole
    {
        None,
        User,
        Admin,
    }

    public class DWUser
    {
        public int UserId { get; set; }

        public string Nickname { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public DWRole[] Roles { get; set; }
    }

    public class DWRole
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DWPermission[] Permissions { get; set; }
    }


    public class DWPermission
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
}

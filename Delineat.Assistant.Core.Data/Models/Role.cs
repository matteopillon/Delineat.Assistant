using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Permission> Permissions { get; set; }

        public ICollection<User> Users { get; set; }
    }

 

   
}

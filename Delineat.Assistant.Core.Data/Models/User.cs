using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
       
        public string Nickname { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<Role> Roles { get; set; }

        public ICollection<UserCredential> Credentials { get; set; }

        public ICollection<DayWorkLog> DayWorkLogs { get; set; }
    }
}

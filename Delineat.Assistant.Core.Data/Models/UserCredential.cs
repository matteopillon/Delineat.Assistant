using System;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class UserCredential
    {
        [Key]
        public int CredentialId { get; set; }

        public string Username { get; set; }


        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        public bool MustChangePassword { get; set; }
        public DateTime PasswordExpiration { get; set; }
    }
}

using Delineat.Assistant.Models;
using System.Collections.Generic;

namespace Delineat.Assistant.Core.Users
{
    public interface IDAUsersStore
    {
        IEnumerable<DWUser> GetUsers();

        DWUser GetUser(int userId);

        bool DeleteUser(int userId);

        bool UpInsertUser(DWUser user);
        
    }
}

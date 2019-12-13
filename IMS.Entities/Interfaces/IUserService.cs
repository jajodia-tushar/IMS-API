using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IUserService
    {
<<<<<<< HEAD

        Task<UsersResponse> GetUsersByRole(String RoleName);

=======
        Task<UsersResponse> GetUsersByRole(String RoleName);
>>>>>>> Changed from taking role id to role name
    }
}

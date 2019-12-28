using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IUserService
    {
        Task<UsersResponse> GetUsersByRole(String roleName);
        Task<UsersResponse> GetAllUsers();
        Task<UsersResponse> GetAllPendingAdminApprovals();
        Task<UsersResponse> AddUser(User user);
    }
}

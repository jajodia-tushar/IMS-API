
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IUserDbContext
    {
        User GetUserByCredintials(string username, string password);
        List<User> GetUsersByRole(string roleName);
        Task<List<User>> GetAllPendingAdminApprovals();
         Task<List<User>> GetAllUsers(Role requestedRole);
         Task<bool> Save(User newUser, int isApproved, int isActive);
        Task<bool> CheckEmailOrUserNameAvailability(string email, string username);
        Task<User> GetUserById(int id);
        Task<User> UpdateUser(User user);
        Task<User> ApproveAdmin(int userId);
        Task<Response> DeleteUser(int userId, bool isHardDelete);
        Task<bool> CheckUserNameAvailability(string username);
    }
}

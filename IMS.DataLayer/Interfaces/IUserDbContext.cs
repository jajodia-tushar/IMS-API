
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IUserDbContext
    {
        Task<User> GetUserByCredintials(string username, string password);
        Task<List<User>> GetUsersByRole(string roleName);
        Task<List<User>> GetAllPendingAdminApprovals();
        Task<List<User>> GetAllUsers(Role requestedRole);
        Task<bool> Save(User newUser, int isApproved, int isActive);
        Task<bool> CheckEmailOrUserNameAvailability(string email, string username);
        Task<User> GetUserById(int id);
        Task<User> UpdateUser(User user);
        Task<User> ApproveAdmin(int userId);
        Task<bool> CheckUserNameAvailability(string username);
        Task<bool> CheckEmailAvailability(string emailId);
        Task<bool> DeleteUser(int userId, bool isHardDelete);
        Task<bool> UpdateUserPassword(int userId, string newHashPassword,string newPassword);
        Task<string> GetOldPassword(int userId);
    }
}

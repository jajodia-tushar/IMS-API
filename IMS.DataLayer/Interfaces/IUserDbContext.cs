
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
    }
}

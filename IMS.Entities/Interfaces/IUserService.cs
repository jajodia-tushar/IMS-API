﻿using System;
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
        Task<UsersResponse> UpdateUser(User userEntity);
        Task<UsersResponse> ApproveAdmin(int userId);

        Task<Response> DeleteUser(int userId, bool isHardDelete);
        Task<Response> CheckUsernameAvailability(string username);
        Task<Response> CheckEmailAvailability(string emailId);
        Task<Response> UpdateUserPassword(int userId, string newPassword);
    }
}

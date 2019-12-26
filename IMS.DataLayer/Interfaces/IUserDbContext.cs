﻿
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
        Task<List<User>> GetUsersByRole(string roleName);
        Task<bool> HasAccessControl(Role requestedRole, Role accessibleRole);
        Task<List<User>> GetAllPendingApprovals(Role requestedRole);

         Task<List<User>> GetAllUsers(Role requestedRole);
    }
}

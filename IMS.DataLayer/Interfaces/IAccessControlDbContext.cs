using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IAccessControlDbContext
    {
        Task<bool> HasAccessControl(Role requestedRole, Role accessibleRole);
    }
}


using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IUserDbContext
    {
        User GetUserByCredintials(string username, string password);
    }
}

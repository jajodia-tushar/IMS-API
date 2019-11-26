using IMS.Entities.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using IMS.DataLayer.Interfaces;

namespace IMS.DataLayer.Dal
{
    public class UserDbContext : IUserDbContext
    {
        public User GetUserByCredintials(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}

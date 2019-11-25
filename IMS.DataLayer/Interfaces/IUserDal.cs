using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IUserDal
    {
        User GetUserByCredintials(string username, string password);
    }
}

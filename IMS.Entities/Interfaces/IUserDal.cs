
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IUserDal
    {
        User GetUserByCredintials(string username, string password);
    }
}

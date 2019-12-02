using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface ILogDbContext
    {
        void Log(Object request,Response response,int userId);
    }
}

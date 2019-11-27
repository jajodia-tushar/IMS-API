using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IEmployeeDbContext
    {
        Employee getEmployeeById(int id);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IEmployeeService 
    {
        GetEmployeeResponse ValidateEmployee(int id);
    }
}

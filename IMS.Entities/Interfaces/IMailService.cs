﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendOrderRecieptToEmployee(EmployeeOrder employeeOrder); 
    }
}

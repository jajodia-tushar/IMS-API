﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class EmployeeBulkOrder
    {
        public Employee Employee { get; set; }
        public EmployeeBulkOrderDetails EmployeeBulkOrderDetails { get; set; }
    }
}

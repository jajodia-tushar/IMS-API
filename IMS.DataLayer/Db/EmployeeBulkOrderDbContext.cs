using IMS.DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class EmployeeBulkOrderDbContext:IEmployeeBulkOrderDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public EmployeeBulkOrderDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
    }
}

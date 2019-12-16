using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class EmployeeOrderDbContext : IEmployeeOrderDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public EmployeeOrderDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        
    }
}

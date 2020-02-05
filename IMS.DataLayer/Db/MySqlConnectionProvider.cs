using IMS.DataLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class MySqlConnectionProvider : IDbConnectionProvider
    {
        public MySqlConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private IConfiguration _configuration;
        public async Task<MySqlConnection> GetConnection(string databaseName)
        {
            throw new NotImplementedException();
        }
    }
}

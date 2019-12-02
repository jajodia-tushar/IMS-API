using IMS.DataLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class MySqlConnectionProvider : IDbConnectionProvider
    {
        public MySqlConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private IConfiguration _configuration;
        public MySqlConnection GetConnection()
        {
            try
            {
                return new MySqlConnection(_configuration["Rds"]);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}

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
        public MySqlConnection GetConnection(string databaseName)
        {
            try
            {
               
               
                
                string server = _configuration["Sql:Server"];
                string port = _configuration["Sql:Port"];
                string username = _configuration["Sql:UserName"];
                string password = _configuration["Sql:Password"];
                string connectionString = $"server={server};port={port};Database={databaseName}; uid={ username };pwd= { password }; convert zero datetime=True;";
                return new MySqlConnection(connectionString);
                
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}

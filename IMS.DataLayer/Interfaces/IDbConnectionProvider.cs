using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IDbConnectionProvider
    {
        Task<MySqlConnection> GetConnection(string databaseName);
    }
}

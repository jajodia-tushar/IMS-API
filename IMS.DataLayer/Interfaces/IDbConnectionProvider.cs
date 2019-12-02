using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IDbConnectionProvider
    {
        MySqlConnection GetConnection(string databaseName);
    }
}

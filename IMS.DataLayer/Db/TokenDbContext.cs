using IMS.DataLayer.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class TokenDbContext : ITokenDbContext
    {
        private IDbConnectionProvider _dbProvider;

        public TokenDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbProvider = dbConnectionProvider;
        }

        public async Task<bool> IsValidToken(string hashToken)
        {
            bool isValid = false;
            MySqlDataReader reader = null;
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetTokenFromHashToken";


                    command.Parameters.AddWithValue("@tokenhash", hashToken);

                    reader = command.ExecuteReader();

                    int rows = 0;
                    while (reader.Read())
                    {
                        rows++;

                    }
                    if (rows == 1)
                        isValid = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return isValid;
        }

            public async Task<bool> StoreToken(string accessToken, string hashToken, DateTime expirationTime)
        {
            bool isStoredToken = false;
            string timestamp = ConvertToString(expirationTime);
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddToken";
                    command.Parameters.AddWithValue("@accesstoken", accessToken);
                    command.Parameters.AddWithValue("@hashtoken", hashToken);
                    command.Parameters.AddWithValue("@expirytimestamp", timestamp);
                    command.ExecuteNonQueryAsync();
                    isStoredToken = true;
                }
                catch(Exception e)
                {
                    throw e;
                }
                return isStoredToken;
            }
        }
        private string ConvertToString(DateTime e)
        {
            return e.Year + "-" + e.Month + "-" + e.Day + " " + e.Hour + ":" + e.Minute + ":" + e.Second;
        }
    }
}

using IMS.DataLayer.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

        public async Task<bool> DeleteToken(string hashToken)
        {
           


            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDeleteToken";
                    command.Parameters.AddWithValue("@hashtoken", hashToken);
                    await command.ExecuteNonQueryAsync();
                }

                catch (Exception ex)
                {
                    return false;
                }

            }
            return true;
        }

        public async Task<bool> IsValidToken(string hashToken)
        {
            bool isValid = false;
            DbDataReader reader = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetTokenFromHashToken";


                    command.Parameters.AddWithValue("@tokenhash", hashToken);

                    reader = await command.ExecuteReaderAsync();

                    
                    while (reader.Read())
                    {
                        isValid = true;

                    }
                  
                       
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
            string timestamp = expirationTime.ToString("yyyy-MM-dd HH:mm:ss");
            using (var connection =await  _dbProvider.GetConnection(Databases.IMS))
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
                    await command.ExecuteNonQueryAsync();
                    isStoredToken = true;
                }
                catch(Exception e)
                {
                    throw e;
                }
                return isStoredToken;
            }
        }
        
    }
}

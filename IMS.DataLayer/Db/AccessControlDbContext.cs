using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class AccessControlDbContext : IAccessControlDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public AccessControlDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }
        public async Task<bool> HasAccessControl(Role requestedRole, Role accessibleRole)
        {
            DbDataReader reader = null;
            bool hasAccess = false;
            using (var connection = _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spHasAccess";
                    command.Parameters.AddWithValue("@requestedroleid", requestedRole.Id);
                    command.Parameters.AddWithValue("@roleidtobeaccessed", accessibleRole.Id);
                    reader = await command.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        hasAccess = (bool)reader["hasaccess"];
                    }



                }

                catch (Exception e)
                {
                    throw e;
                }
            }
            return hasAccess;
        }
    }
}

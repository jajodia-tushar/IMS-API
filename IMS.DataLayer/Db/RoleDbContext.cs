using IMS.DataLayer.Interfaces;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class RoleDbContext : IRoleDbContext
    {
        private IDbConnectionProvider _dbConnectionProvider;
        public RoleDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<List<Role>> GetAccessibleRoles(Role requestedRole)
        {
            DbDataReader reader = null;
            List<Role> roles = new List<Role>();
            using (var connection =await _dbConnectionProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "spGetAllRolesByRequestedRoleId";
                    command.Parameters.AddWithValue("@requestedroleid", requestedRole.Id);
                    command.CommandType = CommandType.StoredProcedure;


                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        roles.Add(new Role
                        {
                            Id = (int)reader["Id"],
                            Name = reader["RoleName"]?.ToString()

                        });
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return roles;
        }
    }
}

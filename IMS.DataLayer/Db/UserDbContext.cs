using IMS.Entities.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using IMS.DataLayer.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Data.Common;
using IMS.Entities.Exceptions;

namespace IMS.DataLayer.Dal
{
    public class UserDbContext : IUserDbContext
    {
        private IDbConnectionProvider _dbProvider;

        public UserDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbProvider = dbConnectionProvider;
        }

        public async Task<List<User>> GetUsersByRole(string roleName)
        {
            List<User> users = new List<User>();
            MySqlDataReader reader = null;

            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetUsersByRole";
                    command.Parameters.AddWithValue("@RoleName", roleName);
                    reader = command.ExecuteReader();
                    User user = null;
                    while (reader.Read())
                    {
                        user = Extract(reader);
                        users.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return users;
        }


        public User GetUserByCredintials(string username, string password)
        {
            User user = null;
            MySqlDataReader reader = null;

            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetUserByCredentials";


                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    reader = command.ExecuteReader();


                    if (reader.Read())
                    {
                        user = Extract(reader);

                    }



                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }


            return user;
        }

        private User Extract(MySqlDataReader reader)
        {
            return new User()
            {
                Id = (int)reader["userid"],
                Username = (string)reader["username"],
                Password = (string)reader["password"],
                Firstname = (string)reader["firstname"],
                Lastname = (string)reader["lastname"],
                Email = (string)reader["email"],
                Role = new Role()
                {
                    Id = (int)reader["roleid"],
                    Name = (string)reader["rolename"]
                }
            };
        }

       
        public async Task<List<User>> GetAllPendingAdminApprovals()
        {
            List<User> users = new List<User>();
            DbDataReader reader = null;

            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllPendingApprovalUsers";
                    reader = await command.ExecuteReaderAsync();
                    User user = null;
                    while (reader.Read())
                    {
                        user = Extract(reader);
                        users.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return users;
        }
        private User Extract(DbDataReader reader)
        {
            return new User()
            {
                Id = (int)reader["userid"],
                Username = (string)reader["username"],
                Password = (string)reader["password"],
                Firstname = (string)reader["firstname"],
                Lastname = (string)reader["lastname"],
                Email = (string)reader["email"],
                Role = new Role()
                {
                    Id = (int)reader["roleid"],
                    Name = (string)reader["rolename"]
                }
            };
        }
    
        public async Task<List<User>> GetAllUsers(Role requestedRole)
        {
            List<User> users = new List<User>();
            DbDataReader reader = null;

            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllUsersByRequestedRole";
                    command.Parameters.AddWithValue("@userroleid", requestedRole.Id);
                    reader = await command.ExecuteReaderAsync();
                    User user = null;
                    while (reader.Read())
                    {
                        user = Extract(reader);
                        users.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return users;
        }
        

        
       
        public async Task<bool> Save(User user, int isApproved, int isActive)
        {
            DbDataReader reader = null;
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddUser";
                    command.Parameters.AddWithValue("@firstname", user.Firstname);
                    command.Parameters.AddWithValue("@lastname", user.Lastname);
                    command.Parameters.AddWithValue("@emailid", user.Email);
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@password", user.Password);
                    command.Parameters.AddWithValue("@roleid", user.Role.Id);
                    command.Parameters.AddWithValue("@isapproved", isApproved);
                    command.Parameters.AddWithValue("@isactive", isActive);

                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        user.Id = (int)reader["generatedid"];
                    }
                    return true;
                }
                catch (CustomException e)
                {
                    throw e;
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == (int)MySqlErrorCode.NoReferencedRow2)
                        throw new CustomException();

                    return false;
                }

            }
        }
       

        public async Task<bool> IsEmailOrUserNameRepeated(string email, string username)
        {
            bool isRepeated = true;
            DbDataReader reader = null;
            using (var connection = _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spIsEmailOrUserNameRepeated";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@emailid", email);
                    reader = await command.ExecuteReaderAsync();
                     if (reader.Read())
                    {
                        isRepeated = (bool)reader["isrepeated"];
                    }
                   
                   
                }
              
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return isRepeated;
        }
    }
}

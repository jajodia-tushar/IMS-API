using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

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
            DbDataReader reader = null;

            using (var connection =await  _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetUsersByRole";
                    command.Parameters.AddWithValue("@RoleName", roleName);
                    reader =await command.ExecuteReaderAsync();
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


        public async Task<User> GetUserByCredintials(string username, string password)
        {
            User user = null;
            DbDataReader reader = null;

            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetUserByCredentials";


                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    reader = await command.ExecuteReaderAsync();


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

        private User Extract(DbDataReader reader)
        {
            return new User()
            {
                Id = (int)reader["userid"],
                Username = reader["username"]?.ToString(),
                Password = reader["password"]?.ToString(),
                Firstname = reader["firstname"]?.ToString(),
                Lastname = reader["lastname"]?.ToString(),
                Email = reader["email"]?.ToString(),
                IsDefaultPasswordChanged=(bool)reader["IsDefaultPasswordChanged"],
                Role = new Role()
                {
                    Id = (int)reader["roleid"],
                    Name = reader["rolename"]?.ToString()
                }
            };
        }
        public async Task<User> GetUserById(int id)
        {
            User user = null;
            DbDataReader reader = null;
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetUserById";
                    command.Parameters.AddWithValue("@Id", id);
                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        user = Extract(reader);

                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            User updatedUser = new User();
            DbDataReader reader = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spUpdateUser";
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@FirstName", user.Firstname);
                    command.Parameters.AddWithValue("@LastName", user.Lastname);
                    command.Parameters.AddWithValue("@EmailId", user.Email);
                    command.Parameters.AddWithValue("@RoleId", user.Role.Id);
                    reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        updatedUser = Extract(reader);
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return updatedUser;
        }


        public async Task<List<User>> GetAllPendingAdminApprovals()
        {
            List<User> users = new List<User>();
            DbDataReader reader = null;

            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
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


        public async Task<List<User>> GetAllUsers(Role requestedRole)
        {
            List<User> users = new List<User>();
            DbDataReader reader = null;

            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
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
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
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


        public async Task<bool> CheckEmailOrUserNameAvailability(string email, string username)
        {
            bool isRepeated = true;
            DbDataReader reader = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
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
        public async Task<User> ApproveAdmin(int userId)
        {
            User user = null;
            DbDataReader reader = null;
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spApproveAdmin";
                    command.Parameters.AddWithValue("@UserId", userId);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
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
        public async Task<bool> DeleteUser(int userId, bool isHardDelete)
        {
            bool isDeleted = false ;
            int rowsAffected = 0;
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDeleteUser";
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@isHardDelete", isHardDelete);
                    rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        isDeleted = true;
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }

            }
            return isDeleted;
        }

        public async Task<bool> CheckUserNameAvailability(string username)
        {
            bool isRepeated = true;
            DbDataReader reader = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spIsUserNamePresent";
                    command.Parameters.AddWithValue("@username", username);
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

        public async Task<bool> CheckEmailAvailability(string emailId)
        {
            bool isRepeated = true;
            DbDataReader reader = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spIsEmailIdPresent";
                    command.Parameters.AddWithValue("@emailId", emailId);
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

        public async Task<bool> UpdateUserPassword(int userId, string newPassword)
        {
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spUpdateUserPassword";
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@newpassword",newPassword);
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                    catch(Exception e)
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }
        public async Task<bool> IsNewpasswordRepeated(int userId,string newPassword)
        {
            bool isRepeated = false;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spIsOldPasswordRepeatAgain";
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@newpassword", newPassword);
                    isRepeated =(bool) await command.ExecuteScalarAsync();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return isRepeated;
            }
        }
    }
}

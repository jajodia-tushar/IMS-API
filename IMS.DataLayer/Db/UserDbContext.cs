using IMS.Entities.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using IMS.DataLayer.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace IMS.DataLayer.Dal
{
    public class UserDbContext : IUserDbContext
    {
        private IConfiguration _configuration;
        public UserDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
       
        public User GetUserByCredintials(string username, string password)
        {
            User user = null;
            MySqlDataReader reader = null;

            using (var connection = new MySqlConnection(_configuration["imsdb"]))
            {
                try
                {
                    
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetUserByCredintials";


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
                Id=(int)reader["userid"],
                Username=(string)reader["username"],
                Password=(string)reader["password"],
                Firstname=(string)reader["firstname"],
                Lastname=(string)reader["lastname"],
                Email=(string)reader["email"],
                Role=new Role()
                {
                    Id=(int)reader["roleid"],
                    Name=(string)reader["rolename"]
                }
            };
        }
    }
}

﻿using IMS.Entities.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using IMS.DataLayer.Interfaces;
using System.Threading.Tasks;

namespace IMS.DataLayer.Dal
{
    public class MockUserDbContext : IUserDbContext
    {
        private static List<User> _users=new List<User>()
            {
               new User()
               {
                   Id =1,
                   Username="admin",
                   Password ="admin123",
                   Firstname ="rochit",
                   Lastname ="aggarwal",
                   Email="rochitaggarwal54@gmail.com",
                   Role =new Role()
                         {
                           Id=4,
                           Name="admin"
                         }
               },
               new User()
               {
                   Id =2,
                   Username="shelf",
                   Password ="shelf123",
                   Firstname ="varsha",
                   Lastname ="vinod",
                   Email="varshavinodl54@gmail.com",
                   Role =new Role()
                         {
                           Id=5,
                           Name="shelf"
                         }
               },
                new User()
               {
                   Id =3,
                   Username="clerk",
                   Password ="clerk123",
                   Firstname ="vijay",
                   Lastname ="mohan",
                   Email="vijaymohan54@gmail.com",
                   Role =new Role()
                         {
                           Id=6,
                           Name="clerk"
                         }
               }

            };

        public Task<List<User>> GetAllUsers(Role requestedRole)
        {
            throw new NotImplementedException();
        }
       

        public User GetUserByCredintials(string username, string password)
        {
            return _users.Find
                   (
                        u =>
                        {
                           return u.Username.Equals(username) && u.Password.Equals(password);
                        }
                   );
        }

        public Task<List<User>> GetUsersByRole(string RoleName)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAllPendingAdminApprovals()
        {
            throw new NotImplementedException();
        }
    }
}

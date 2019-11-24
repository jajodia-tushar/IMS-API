using IMS.Contracts.Models;
using IMS.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dal
{ 
    public class UserDal
    {
        private List<UserDto> _users = new List<UserDto>()
        { 
           new UserDto()
           {
               UserId =1,
               Username="raggarwal",
               Password ="raggarwal",
               Firstname ="rochit",
               Secondname ="aggarwal",
               Email="rochitaggarwal54@gmail.com",
               Role ="user"
           },
            new UserDto()
           {
               UserId =2,
               Username="varsha",
               Password ="varsha",
               Firstname ="varsha",
               Secondname ="Vinod",
               Email="varshavinod54@gmail.com",
               Role ="admin"
           }
        };

        public User GetUserByUsername(string username,string password)
        {
            var userModel = _users.Find(user => user.Username.Equals(username) && user.Password.Equals(password));
            if(userModel!=null)
            {
                return new User(userModel.UserId, userModel.Username, userModel.Firstname, userModel.Secondname, userModel.Role);
            }
            return null;
        }
    }
}

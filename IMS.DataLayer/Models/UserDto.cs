using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Models
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string  Password { get; set; }
        public string Firstname { get; set; }
        public string Secondname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts.Models
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string Fullname { get; private set; }
        public string Role { get; private set; }
        public User(int id,string username,string firstname,string lastname,string role)
        {
            Id = id;
            Username = username;
            Fullname = firstname+lastname;
            Role = role;
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Secondname { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }


    }
}

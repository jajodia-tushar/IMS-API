using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Db
{
    public class MockEmployeeDbContext : IEmployeeDbContext
    {
        private static List<Employee> employees = new List<Employee>()
            {
               new Employee()
               {
                   Id =1126,
                   Firstname ="Rochit",
                   Lastname ="Aggarwal",
                   Email="rochitaggarwal54@gmail.com",
                   MobileNumber="1234567890",
                   TCardNo="",
                   AccessCardNo="",
                   IsActive=true
               },
               new Employee()
               {
                   Id = 302,
                   Firstname ="Dhvani",
                   Lastname ="Sheth",
                   Email="dshethl54@gmail.com",
                   MobileNumber="0987654321",
                   TCardNo="302",
                   AccessCardNo="302",
                   IsActive=true
               },
                new Employee()
               {
                   Id =1129,
                   Firstname ="Vijay",
                   Lastname ="Mohan",
                   Email="vijayl54@gmail.com",
                   MobileNumber="53628101012",
                   TCardNo="",
                   AccessCardNo="",
                   IsActive=true
               }
            };
        public Employee getEmployeeById(int id)
        {
            return employees.Find
                   (
                        u =>
                        {
                            return u.Id.Equals(id) ;
                        }
                   );
        }
    }
}

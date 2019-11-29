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
                   ContactNumber="1234567890",
                   TCardNumber="",
                   AccessCardNumber="",
                   IsActive=true
               },
               new Employee()
               {
                   Id = 302,
                   Firstname ="Dhvani",
                   Lastname ="Sheth",
                   Email="dshethl54@gmail.com",
                   ContactNumber="0987654321",
                   TCardNumber="302",
                   AccessCardNumber="302",
                   IsActive=true
               },
                new Employee()
               {
                   Id =1129,
                   Firstname ="Vijay",
                   Lastname ="Mohan",
                   Email="vijayl54@gmail.com",
                   ContactNumber="53628101012",
                   TCardNumber="",
                   AccessCardNumber="",
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

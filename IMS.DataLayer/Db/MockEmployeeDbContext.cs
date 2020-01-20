using IMS.DataLayer.Interfaces;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Db
{
    public class MockEmployeeDbContext : IEmployeeDbContext
    {
        private static List<Employee> employees = new List<Employee>()
            {
               new Employee()
               {
                   Id ="1126",
                   Firstname ="Rochit",
                   Lastname ="Aggarwal",
                   Email="rochitaggarwal54@gmail.com",
                   ContactNumber="1234567890",
                   TemporaryCardNumber="",
                   AccessCardNumber="",
                   IsActive=true
               },
               new Employee()
               {
                   Id = "302",
                   Firstname ="Dhvani",
                   Lastname ="Sheth",
                   Email="dshethl54@gmail.com",
                   ContactNumber="0987654321",
                   TemporaryCardNumber="302",
                   AccessCardNumber="302",
                   IsActive=true
               },
                new Employee()
               {
                   Id ="1129",
                   Firstname ="Vijay",
                   Lastname ="Mohan",
                   Email="vijayl54@gmail.com",
                   ContactNumber="53628101012",
                   TemporaryCardNumber="",
                   AccessCardNumber="",
                   IsActive=true
               }
            };

        

        public Task<bool> CheckEmpEmailAvailability(string email)
        {
            throw new NotImplementedException();
        }


        public Task<EmployeeResponse> GetAllEmployees(string filter, int limit, int offset)
        {
            throw new NotImplementedException();
        }

        Task<string> IEmployeeDbContext.CreateEmployee(Employee employee)
        public Task<bool> CheckEmployeeIdAvailability(string employeeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string employeeId, bool isHardDelete)
        {
            throw new NotImplementedException();
        }

        public Task<Employee> Update(Employee updatedEmployee)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmployeeDetailsRepetitionCheck(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<Employee> GetEmployeeById(string employeeId)
        {
            throw new NotImplementedException();
        }
    }
}

using IMS.DataLayer.Db;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.AddEmployees
{
    public class Program
    {
        public static void Main(string[] args)
        {   
            var builder = new ConfigurationBuilder()
               .SetBasePath(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..")))
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            var services = new ServiceCollection().AddTransient<AddEmployeesData>()
                .AddSingleton(typeof(IConfiguration), configuration)
                .AddTransient<IDbConnectionProvider, SshSqlDbConnectionProvider>()
                .AddTransient<IEmployeeDbContext, EmployeeDbContext>(); 
            var provider = services.BuildServiceProvider();
            var container = provider.GetService<AddEmployeesData>();
            container.AddEmployeesFromCsvFile();
            Console.ReadLine();
        }
    }
}

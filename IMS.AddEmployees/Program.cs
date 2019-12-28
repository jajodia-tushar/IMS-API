using IMS.DataLayer.Db;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.EmployeeDataDumper
{
    public class Program
    {
        public static void Main(string[] args)
        {   
            var builder = new ConfigurationBuilder()
               .SetBasePath(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..")))
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            var services = new ServiceCollection().AddTransient<EmployeesDataInjector>()
                .AddSingleton(typeof(IConfiguration), configuration)
                .AddTransient<IDbConnectionProvider, SshSqlDbConnectionProvider>()
                .AddTransient<IEmployeesDataDbContext,EmployeesDataDbContext>()
                .AddTransient<ILogManager, LogImplementation>()
                .AddTransient<ILogDbContext, LogDbContext>();
            var provider = services.BuildServiceProvider();
            var container = provider.GetService<EmployeesDataInjector>();
            container.AddEmployeesFromCsvFile();
            Console.ReadLine();
        }
    }
}

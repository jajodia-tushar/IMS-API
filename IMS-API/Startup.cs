using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS.Core.services;
using IMS.DataLayer.Dal;
using IMS.DataLayer;
using IMS.Entities.Interfaces;
using IMS.TokenManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using IMS.DataLayer.Interfaces;
using IMS.Logging;
using IMS.DataLayer.Db;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using SimpleProxy.Extensions;
using IMS.Entities;
using IMS.Core;

namespace IMS_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "IMS API",
                    Version = "v1",
                    Description = "Inventory Management Service Web APIs",
                    Contact = new Contact() { Name = "COD Team", Email = "raggarwal@tavisca.com", Url = "" },
                });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "IMS-API.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = CreateTokenValidationParameters();
                    
                });


            services.AddTransient<IUserDbContext, UserDbContext>();
            services.AddTransient<ITokenProvider, JwtTokenProvider>();
            services.AddTransient<ILogManager, LogImplementation>();
            services.AddTransient<ILogDbContext, LogDbContext>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<ITokenDbContext, TokenDbContext>();
            services.AddTransient<IEmployeeDbContext, EmployeeDbContext>();
            services.AddTransient<IDbConnectionProvider, SshSqlDbConnectionProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IShelfDbContext,ShelfDbContext>();
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IInventoryDbContext, InventoryDbContext>();
            services.AddTransient<IVendorDbContext, VendorDbContext>();
            services.AddTransient<IVendorOrderDbContext, VendorOrderDbContext>();
            services.AddTransient<IEmployeeOrderDbContext, EmployeeOrderDbContext>();
            services.AddTransient<IVendorOrderDbContext, VendorOrderDbContext>();
            services.AddTransient<IReportsDbContext, ReportsDbContext>();
            services.AddTransient<IReportsService, ReportsService>();
            services.AddTransient<IVendorOrderDbContext, VendorOrderDbContext>();
            services.AddTransient<IItemDbContext, ItemDbContext>();
            services.AddTransient<IVendorOrderDbContext, VendorOrderDbContext>();
            services.AddTransient<ITransferDbContext, TransferDbContext>();
            services.AddTransient<ITransferService, TransferService>();
            services.AddTransient<IAccessControlDbContext, AccessControlDbContext>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IRoleDbContext, RoleDbContext>();
            services.AddTransient<ILogsService, LogsService>();
            services.AddTransient<INotificationProvider, NotificationProvider>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IEmployeeBulkOrderDbContext, EmployeeBulkOrderDbContext>();
            services.AddTransient<IAdminNotificationService, AdminNotificationService>();
            services.AddTransient<IAdminNotificationDbContext, AdminNotificationDbContext>();

            services.AddTransient<IAuditLogsDbContext, AuditLogDbContext>();
            services.AddTransient<IEmployeeBulkOrderDbContext, EmployeeBulkOrderDbContext>();
            services.EnableSimpleProxy(p => p.AddInterceptor<AuditAttribute, AuditInterceptor>());
            services.AddTransientWithProxy<IItemService, ItemService>();
            services.AddTransientWithProxy<IShelfService, ShelfService>();
            services.AddTransientWithProxy<IEmployeeService, EmployeeService>();
            services.AddTransientWithProxy<IUserService, UserService>();
            services.AddTransientWithProxy<IVendorService, VendorService>();
            services.AddTransientWithProxy<IOrderService, OrderService>();

        }
        public TokenValidationParameters CreateTokenValidationParameters()
        {
            var result = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = "noissuer",

                ValidateAudience = false,
                ValidAudience = "noaudience",

                ValidateIssuerSigningKey = false,
               
                SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                {
                    JwtSecurityToken jwt = null;
                    try
                    {
                         jwt = new JwtSecurityToken(token);
                        
                    }
                    catch(Exception e)
                    {

                    }

                    return jwt;
                },

                RequireExpirationTime = false,
                ValidateLifetime = false,

               
            };

            result.RequireSignedTokens = false;

            return result;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>

            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMS API Version 1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}

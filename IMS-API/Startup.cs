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

            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IEmployeeDbContext, EmployeeDbContext>();
            services.AddTransient<IDbConnectionProvider, SshSqlDbConnectionProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IInventoryService, InventoryService>();
            services.AddTransient<IShelfService, ShelfService>();
            services.AddTransient<IShelfDbContext,ShelfDbContext>();
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IInventoryDbContext, InventoryDbContext>();
            services.AddTransient<IVendorService, VendorService>();
            services.AddTransient<IVendorDbContext, VendorDbContext>();
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
                    var jwt = new JwtSecurityToken(token);

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

using FluentMigrator.Runner;
using Identity.Dapper.Models;
using Identity.Dapper.MySQL.Connections;
using Identity.Dapper.MySQL.Models;

using Net.Core.Web.Entities;
using Net.Core.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.Core.DAO;
using Net.Core.DAO.Migrations;
using System;
using Identity.Dapper;
using Net.Core.Common;
using Net.Core.Common.Interfaces;
using Net.Core.Services.Implementation;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using net.core.Samples.Web;

namespace Net.Core.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //.AddNewtonsoftJson(options => { options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc; options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"; options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddConsole();
                builder.AddEventSourceLogger();
                builder.AddFile(Configuration.GetSection("Logging"));
            });
            loggerFactory.AddFile(Configuration.GetSection("Logging"));
            loggerFactory.CreateLogger<Startup>();
            //services.ConfigureDapperConnectionProvider<SqlServerConnectionProvider>(Configuration.GetSection("DapperIdentity"))
            //        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
            //        .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            services.ConfigureDapperConnectionProvider<MySqlConnectionProvider>(Configuration.GetSection("DapperIdentity"))
                    .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
                    .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            //services.ConfigureDapperConnectionProvider<PostgreSqlConnectionProvider>(Configuration.GetSection("ConnectionStrings"))
            //services.ConfigureDapperConnectionProvider<PostgreSqlConnectionProvider>(Configuration.GetSection("DapperIdentity"))
            //        .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"))
            //        .ConfigureDapperIdentityOptions(new DapperIdentityOptions { UseTransactionalBehavior = false }); //Change to True to use Transactions in all operations

            services.AddIdentity<CustomUser, CustomRole>(x =>
                                                         {
                                                             x.Password.RequireDigit = false;
                                                             x.Password.RequiredLength = 1;
                                                             x.Password.RequireLowercase = false;
                                                             x.Password.RequireNonAlphanumeric = false;
                                                             x.Password.RequireUppercase = false;
                                                         })
                    // .AddDapperIdentityFor<PostgreSqlConfiguration>()
                    //.AddDapperIdentityFor<SqlServerConfiguration>()
                    .AddDapperIdentityFor<MySqlConfiguration>()
                    .AddDefaultTokenProviders();
            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            // })
            //.AddJwtBearer(options =>
            //{
            //    options.SaveToken = true;
            //    options.RequireHttpsMetadata = false;
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
            //        ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value)),
            //        ClockSkew = TimeSpan.Zero
            //    };

            //});
            services.AddFluentMigratorCore()
                 .ConfigureRunner(rb => rb
                     .AddMySql5()

                     .WithGlobalConnectionString(Configuration.GetSection("DapperIdentity:GlobalConnectionString").Value)
                 // Define the assembly containing the migrations
                 .ScanIn(typeof(Tables).Assembly).For.Migrations());
            // Enable logging to console in the FluentMigrator way
            // AddLogging(lb => lb.AddFluentMigratorConsole()
            services.AddSingleton<DapperContext>();

            services.AddAuthorization();
            services.AddMvc(m => m.EnableEndpointRouting = false);

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddScoped<ICustomer, CustomerService>();
            services.AddScoped<ITransaction, TransactionService>();

            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<SchemaFilter>();
                c.CustomSchemaIds(i => i.FullName);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Sql Api", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Migrate();
            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity"));
            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            Application.ServiceProvider = app.ApplicationServices;
        }

    }

}

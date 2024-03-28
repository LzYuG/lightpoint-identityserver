using Hangfire;
using Hangfire.PostgreSql;
using LightPoint.IdentityServer.QuickStart.Foundation.LightPointIdentityServer.ServiceImps;
using LightPoint.IdentityServer.QuickStart.Server.ORM;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Contexts;
using LightPoint.IdentityServer.ServerInfrastructure.ServiceInjectConfig;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace LightPoint.IdentityServer.QuickStart.Foundation.LightPointIdentityServer
{
    public class ServiceImpFactoryImp : ServiceImpFactoryBase, IServiceImpFactory
    {
        public override void AddDataContext(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var migrationsAssembly = typeof(LPIdentityServerDbContext).GetTypeInfo().Assembly.GetName().Name;
            // 默认注入DbContext
            string connectionString = "";

            if (environment.IsDevelopment())
            {
                connectionString = configuration.GetConnectionString("DBConnectionStr")!;
            }
            else
            {
                var sqlString = Environment.GetEnvironmentVariable("SQLConnectionStr", EnvironmentVariableTarget.Process);
                if (string.IsNullOrEmpty(sqlString))
                {
                    throw new Exception("未配置链接字符串");
                }
                connectionString = sqlString;
            }

            services.AddScoped<DbContext, LPIdentityServerDbContext>();

            // 使用SQL Server
            //services.AddDbContext<CddIdentityServerDbContext>(options => options
            //        .UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            // 使用PG SQL
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", false);
            services.AddDbContext<LPIdentityServerDbContext>(options => options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)), ServiceLifetime.Scoped);
            //services.AddDbContext<ConfigurationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityContext")));

            // 在Blazor Server，应使用DbContextFactory
            services.AddDbContextFactory<LPIdentityServerDbContext>(lifetime: ServiceLifetime.Scoped);
            services.AddScoped<ILightPointDbContextFactory, LightPointDbContextFactory>();
        }

        public override void AddIdentity(IServiceCollection services)
        {
            // 无需配置规则，相关规则由Ids处理
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddEntityFrameworkStores<DbContext>()
                .AddDefaultTokenProviders();
        }

        public override Type GetApplicationMenuStoreImplementType()
        {
            return typeof(ApplicationMenuStore);
        }

        public override Type GetConfidentialServiceImplementType()
        {
            return typeof(LightPointAesConfidentialService);
        }
    }
}

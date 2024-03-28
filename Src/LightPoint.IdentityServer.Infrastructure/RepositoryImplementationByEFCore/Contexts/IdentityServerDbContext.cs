using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.Shared;
using Microsoft.EntityFrameworkCore;

namespace LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Contexts
{
    public abstract class IdentityServerDbContext : DbContext
    {
        public IdentityServerDbContext(DbContextOptions options) : base(options)
        { 

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IData<Guid>).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }
            base.OnModelCreating(modelBuilder);
        }


        #region DM01.SystemResources
        public DbSet<SystemCommonFile> SystemCommonFiles { get; set; }
        public DbSet<ServerCommonConfig> ServerCommonConfigs { get; set; }
        public DbSet<ServerEmailConfig> ServerEmailConfigs { get; set; }
        public DbSet<SystemAccountConfig> SystemAccountConfigs { get; set; }
        public DbSet<ServerShortMessageServiceConfig> ServerShortMessageServiceConfigs { get; set; }
        public DbSet<SystemTenant> SystemTenants { get; set; }
        #endregion

        #region DM02.ApplicationIdentityResources
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserClaim> ApplicationUserClaims { get; set; }
        public DbSet<ApplicationUserWithRole> ApplicationUserWithRoles { get; set; }
        #endregion

        #region DM03.IdentityServerResources
        public DbSet<IdentityServerApiResource> IdentityServerApiResources { get; set; }
        public DbSet<IdentityServerApiScope> IdentityServerApiScopes { get; set; }
        public DbSet<IdentityServerClient> IdentityServerClients { get; set; }
        public DbSet<IdentityServerIdentityResource> IdentityServerIdentityResources { get; set; }
        public DbSet<IdentityServerDeviceFlowCode> IdentityServerDeviceFlowCodes { get; set; }
        public DbSet<IdentityServerPersistedGrant> IdentityServerPersistedGrants { get; set; }
        #endregion

        #region DM04.LogAuditingResources
        public DbSet<ApplicationUserLoginedLog> ApplicationUserLoginedLogs { get; set; }
        public DbSet<MessageSendedLog> MessageSendedLogs { get; set; }
        public DbSet<ServerRunningLog> ServerRunningLog { get; set; }
        #endregion
    }
}

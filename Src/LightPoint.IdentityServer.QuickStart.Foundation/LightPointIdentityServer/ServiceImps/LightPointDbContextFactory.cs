using LightPoint.IdentityServer.QuickStart.Server.ORM;
using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LightPoint.IdentityServer.QuickStart.Foundation.LightPointIdentityServer.ServiceImps
{
    public class LightPointDbContextFactory : ILightPointDbContextFactory
    {
        private readonly IDbContextFactory<LPIdentityServerDbContext> _dbContextFactory;

        public LightPointDbContextFactory(IDbContextFactory<LPIdentityServerDbContext> dbContextFactory) 
        {
            _dbContextFactory = dbContextFactory;
        }

        public DbContext CreateDbContext()
        {
            return _dbContextFactory.CreateDbContext();
        }

        public async Task<DbContext> CreateDbContextAsync()
        {
            return await _dbContextFactory.CreateDbContextAsync();
        }
    }
}

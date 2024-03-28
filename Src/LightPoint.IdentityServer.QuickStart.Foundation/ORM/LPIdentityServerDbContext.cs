using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LightPoint.IdentityServer.QuickStart.Server.ORM
{
    public class LPIdentityServerDbContext : IdentityServerDbContext
    {
        public LPIdentityServerDbContext(DbContextOptions<LPIdentityServerDbContext> options) : base(options)
        {
        }
    }
}
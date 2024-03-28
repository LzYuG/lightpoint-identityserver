using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Contexts
{
    public interface ILightPointDbContextFactory
    {
        Task<DbContext> CreateDbContextAsync();
        DbContext CreateDbContext();
    }
}

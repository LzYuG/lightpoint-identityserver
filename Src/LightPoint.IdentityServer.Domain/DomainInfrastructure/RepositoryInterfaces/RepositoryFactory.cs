using LightPoint.IdentityServer.Domain.DomainModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces
{
    public class RepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommandRepository<Tid, TDomainModel> GetCommandRepository<Tid, TDomainModel>()
           where TDomainModel : class, IDomainModel<Tid>, new()
        {
            return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ICommandRepository<Tid, TDomainModel>>();
        }

        public IQueryRepository<Tid, TDomainModel> GetQueryRepository<Tid, TDomainModel>()
            where TDomainModel : class, IDomainModel<Tid>, new()
        {
            return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IQueryRepository<Tid, TDomainModel>>();
        }
    }
}

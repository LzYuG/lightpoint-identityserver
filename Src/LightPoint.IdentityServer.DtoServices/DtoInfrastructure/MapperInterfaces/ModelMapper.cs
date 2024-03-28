using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces
{
    /// <summary>
    /// ModelMapper默认实现类
    /// </summary>
    public class ModelMapper<Tid, TDomain, TQuery, TCommand> : IModelMapper<Tid, TDomain, TQuery, TCommand>
        where TDomain : class, IDomainModelBase<Tid>, new()
        where TQuery : class, IQueryDtoBase<Tid>, new()
        where TCommand : class, ICommandDtoBase<Tid>, new()
    {
        private readonly IConfidentialService _confidentialService;

        public ModelMapper(IConfidentialService confidentialService)
        {
            _confidentialService = confidentialService;
        }

        public virtual async Task<TDomain?> ToDomainModel(TCommand command)
        {
            command = await _confidentialService.EncryptModelConfidentialPropsAsync(command);
            return Mapper<TCommand, TDomain>.MapToNewObj(command!);
        }

        public virtual async Task<List<TDomain>> ToDomainModel(IEnumerable<TCommand> commands)
        {
            var result = new List<TDomain>();
            foreach(var command in commands)
            {
                var temp = await ToDomainModel(command);
                result.Add(temp!);
            }
            return result;
        }

        public virtual Task<TQuery?> ToQueryDto(TDomain domain)
        {
            return Task.FromResult(Mapper<TDomain, TQuery>.MapToNewObj(domain));
        }

        public virtual async Task<List<TQuery>> ToQueryDto(IEnumerable<TDomain> domains)
        {
            var result = new List<TQuery>();
            foreach (var domain in domains)
            {
                var temp = await ToQueryDto(domain);
                result.Add(temp!);
            }
            return result;
        }
    }
}

using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces
{
    public interface IModelMapper<Tid, TDomain, TQuery, TCommand>
        where TDomain : class, IDomainModelBase<Tid>, new()
        where TQuery : class, IQueryDtoBase<Tid>, new()
        where TCommand : class, ICommandDtoBase<Tid>, new()
    {
        /// <summary>
        /// CommandDto To DomainModel
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<TDomain?> ToDomainModel(TCommand command);
        /// <summary>
        /// CommandDtos To DomainModels
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<TDomain>> ToDomainModel(IEnumerable<TCommand> commands);
        /// <summary>
        /// DomainModel To QueryDto
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        Task<TQuery?> ToQueryDto(TDomain domain);
        /// <summary>
        /// DomainModels To QueryDtos
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        Task<List<TQuery>> ToQueryDto(IEnumerable<TDomain> domains);
    }
}

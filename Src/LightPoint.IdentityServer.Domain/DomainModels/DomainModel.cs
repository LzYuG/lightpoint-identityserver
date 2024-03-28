using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using System.ComponentModel.DataAnnotations;

namespace LightPoint.IdentityServer.Domain.DomainModels
{
    /// <summary>
    /// 对于IEntityBase的实现
    /// </summary>
    public abstract class DomainModel<T> : DomainModelBase<T>, IDomainModel<T>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}

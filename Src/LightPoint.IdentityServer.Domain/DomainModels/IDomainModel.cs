using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Shared;

namespace LightPoint.IdentityServer.Domain.DomainModels
{
    /// <summary>
    /// 实体模型基架接口，所有实体模型都将实现这个接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDomainModel<T> : IDomainModelBase<T>
    {
        /// <summary>
        /// 名字
        /// </summary>
        string? Name { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        string? Description { get; set; }
    }
}

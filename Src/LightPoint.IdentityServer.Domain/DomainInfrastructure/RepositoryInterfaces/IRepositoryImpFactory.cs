using LightPoint.IdentityServer.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces
{
    public interface IRepositoryImpFactory
    {
        /// <summary>
        /// 获取默认的Query仓储的实现类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <returns></returns>
        Type GetQueryRepositoryImplementType<Tid, TDomain>()
            where TDomain : class, IDomainModelBase<Tid>, new();

        /// <summary>
        /// 获取默认的Command仓储的实现类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <returns></returns>
        Type GetCommandRepositoryImplementType<Tid, TDomain>()
            where TDomain : class, IDomainModelBase<Tid>, new();
    }
}

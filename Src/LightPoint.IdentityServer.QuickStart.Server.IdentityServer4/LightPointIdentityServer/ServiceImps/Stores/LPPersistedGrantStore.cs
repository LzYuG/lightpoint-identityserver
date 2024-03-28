using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.Shared.Helpers;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.Stores
{
    public class LPPersistedGrantStore : IPersistedGrantStore
    {
        private readonly IAppService<Guid, IdentityServerPersistedGrant, IdentityServerPersistedGrantDM, IdentityServerPersistedGrantDM> _identityServerPersistedGrantService;
        private readonly TenantInfoAccessor _tenantInfoAccessor;

        public LPPersistedGrantStore(IAppService<Guid, IdentityServerPersistedGrant, IdentityServerPersistedGrantDM, IdentityServerPersistedGrantDM> identityServerPersistedGrantService,
            TenantInfoAccessor tenantInfoAccessor)
        {
            _identityServerPersistedGrantService = identityServerPersistedGrantService;
            _tenantInfoAccessor = tenantInfoAccessor;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var expression = _BindFilterExpression(filter);
            var res = await _identityServerPersistedGrantService.GetApiBoCollectionAsync(expression, x => x.SortCode!, true);
            return Mapper<IdentityServerPersistedGrantDM, PersistedGrant>.MapToNewObj(res!)!;
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var res = await _identityServerPersistedGrantService.GetApiBoAsync(x => x.Key == key && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
            return Mapper<IdentityServerPersistedGrantDM, PersistedGrant>.MapToNewObj(res!)!;
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            var expression = _BindFilterExpression(filter);
            await _identityServerPersistedGrantService.DeleteBoAsync(expression);
        }

        public async Task RemoveAsync(string key)
        {
            await _identityServerPersistedGrantService.DeleteBoAsync(x => x.Key == key && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var will = Mapper<PersistedGrant, IdentityServerPersistedGrantDM>.MapToNewObj(grant);
            will!.TenantIdentifier = _tenantInfoAccessor.TenantIdentifier;
            await _identityServerPersistedGrantService.SetAndSaveEntityData(will!, true);
        }


        private Expression<Func<IdentityServerPersistedGrant, bool>> _BindFilterExpression(PersistedGrantFilter filter)
        {
            filter.Validate();
            Expression<Func<IdentityServerPersistedGrant, bool>> expression = x => x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier;
            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                expression = expression.ExpressionAnd(x => x.ClientId == filter.ClientId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                expression = expression.ExpressionAnd(x => x.SessionId == filter.SessionId);
            }
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                expression = expression.ExpressionAnd(x => x.SubjectId == filter.SubjectId);
            }
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                expression = expression.ExpressionAnd(x => x.Type == filter.Type);
            }
            return expression;
        }
    }
}

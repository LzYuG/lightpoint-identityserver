using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;
using IdentityServer4.Stores.Serialization;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.Stores
{
    public class LPDeviceFlowStore : IDeviceFlowStore
    {
        private readonly IAppService<Guid, IdentityServerDeviceFlowCode, IdentityServerDeviceFlowCodeDM, IdentityServerDeviceFlowCodeDM> _identityServerDeviceFlowCodeService;
        private readonly ILogger<LPDeviceFlowStore> _logger;
        private readonly TenantInfoAccessor _tenantInfoAccessor;
        private readonly IPersistentGrantSerializer _serializer;

        public LPDeviceFlowStore(IAppService<Guid, IdentityServerDeviceFlowCode, IdentityServerDeviceFlowCodeDM, IdentityServerDeviceFlowCodeDM> identityServerDeviceFlowCodeService,
            ILogger<LPDeviceFlowStore> logger, TenantInfoAccessor tenantInfoAccessor, IPersistentGrantSerializer serializer)
        {
            _identityServerDeviceFlowCodeService = identityServerDeviceFlowCodeService;
            _logger = logger;
            _tenantInfoAccessor = tenantInfoAccessor;
            _serializer = serializer;
        }

        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            var res = await _identityServerDeviceFlowCodeService.GetApiBoAsync(x => x.DeviceCode == deviceCode && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
            if (res == null)
            {
                return null!;
            }
            var desRes = _serializer.Deserialize<DeviceCode>(res!.Data!)!;
            return desRes;
        }

        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            var res = await _identityServerDeviceFlowCodeService.GetApiBoAsync(x => x.UserCode == userCode && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
            if(res == null)
            {
                return null!;
            }
            var desRes = _serializer.Deserialize<DeviceCode>(res!.Data!)!;
            return desRes;
        }

        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            await _identityServerDeviceFlowCodeService.DeleteBoAsync(x => x.DeviceCode == deviceCode && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
        }

        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            if (data == null || deviceCode == null || userCode == null) return;
            var will = new IdentityServerDeviceFlowCodeDM
            {
                DeviceCode = deviceCode,
                UserCode = userCode,
                TenantIdentifier = _tenantInfoAccessor.TenantIdentifier,
                ClientId = data.ClientId,
                SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject)!.Value,
                CreationTime = data.CreationTime,
                Expiration = data.CreationTime.AddSeconds(data.Lifetime),
                Data = _serializer.Serialize(data),
            };

            await _identityServerDeviceFlowCodeService.SetAndSaveEntityData(will, true);
        }

        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            var existing = await _identityServerDeviceFlowCodeService.GetApiBoAsync(x => x.UserCode == userCode && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
            if (existing == null)
            {
                _logger.LogError("{userCode} not found in database", userCode);
                throw new InvalidOperationException("Could not update device code");
            }
            _logger.LogDebug("{userCode} found in database", userCode);

            existing.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject)!.Value;
            existing.Data = _serializer.Serialize(data);

            try
            {
                await _identityServerDeviceFlowCodeService.SetAndSaveEntityData(existing, true);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning("exception updating {userCode} user code in database: {error}", userCode, ex.Message);
            }
        }
    }
}

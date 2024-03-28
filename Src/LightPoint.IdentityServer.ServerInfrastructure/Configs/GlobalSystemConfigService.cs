using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Configs
{
    public class GlobalSystemConfigService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppService<Guid, ServerEmailConfig, ServerEmailConfigDQM, ServerEmailConfigDCM> _serverEmailConfigService;
        private readonly IAppService<Guid, ServerShortMessageServiceConfig, ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM> _serverShortMessageServiceConfigService;
        private readonly IAppService<Guid, SystemAccountConfig, SystemAccountConfigDQM, SystemAccountConfigDCM> _systemAccountConfigService;
        private readonly IAppService<Guid, ServerCommonConfig, ServerCommonConfigDQM, ServerCommonConfigDCM> _serverCommonConfigService;

        private TenantInfo TenantInfo { get; set; }

        public GlobalSystemConfigService(IHttpContextAccessor httpContextAccessor,
            IAppService<Guid, ServerEmailConfig, ServerEmailConfigDQM, ServerEmailConfigDCM> serverEmailConfigService,
            IAppService<Guid, ServerShortMessageServiceConfig, ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM> serverShortMessageServiceConfigService,
            IAppService<Guid, SystemAccountConfig, SystemAccountConfigDQM, SystemAccountConfigDCM> systemAccountConfigService,
            IAppService<Guid, ServerCommonConfig, ServerCommonConfigDQM, ServerCommonConfigDCM> serverCommonConfigService)
        {
            _httpContextAccessor = httpContextAccessor;
            _serverEmailConfigService = serverEmailConfigService;
            _serverShortMessageServiceConfigService = serverShortMessageServiceConfigService;
            _systemAccountConfigService = systemAccountConfigService;
            _serverCommonConfigService = serverCommonConfigService;
            TenantInfo = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<TenantInfo>();
        }


        public async Task<ServerEmailConfigDQM?> GetServerEmailConfig()
        {
            return await _serverEmailConfigService.GetApiBoAsync(x => x.TenantIdentifier == TenantInfo.TenantIdentifier, true);
        }

        public async Task<ServerCommonConfigDQM?> GetServerCommonConfig()
        {
            return await _serverCommonConfigService.GetApiBoAsync(x => x.TenantIdentifier == TenantInfo.TenantIdentifier, true);
        }

        public async Task<SystemAccountConfigDQM?> GetSystemAccountConfig()
        {
            return await _systemAccountConfigService.GetApiBoAsync(x => x.TenantIdentifier == TenantInfo.TenantIdentifier, true);
        }

        public async Task<ServerShortMessageServiceConfigDQM?> GetServerShortMessageServiceConfig()
        {
            return await _serverShortMessageServiceConfigService.GetApiBoAsync(x => x.TenantIdentifier == TenantInfo.TenantIdentifier, true);
        }
    }
}

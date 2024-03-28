using AntDesign;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.ServerInfrastructure.ValidationCode;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.ResourcesOperationMaintenanceBusiness.SystemResourcesBusiness
{
    partial class SystemGlobalConfigPage
    {
        public ServerEmailConfigDCM? ServerEmailConfig { get; set; }
        public ServerShortMessageServiceConfigDCM? ServerShortMessageServiceConfig { get; set; }
        public SystemAccountConfigDCM? SystemAccountConfig { get; set; }
        public ServerCommonConfigDCM? ServerCommonConfig { get; set; }

        [CascadingParameter]
        public SystemTenantDQM NowSystemTenant { get; set; } = new SystemTenantDQM() { TenantIdentifier = "" };

        #region Inject Services
        [Inject]
        public IAppService<Guid, ServerEmailConfig, ServerEmailConfigDQM, ServerEmailConfigDCM>? ServerEmailConfigService { get; set; }
        [Inject]
        public IAppService<Guid, ServerShortMessageServiceConfig, ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM>? ServerShortMessageServiceConfigService { get; set; }
        [Inject]
        public IAppService<Guid, SystemAccountConfig, SystemAccountConfigDQM, SystemAccountConfigDCM>? SystemAccountConfigService { get; set; }
        [Inject]
        public IAppService<Guid, ServerCommonConfig, ServerCommonConfigDQM, ServerCommonConfigDCM>? ServerCommonConfigService { get; set; }
        [Inject]
        public IMessageService? MessageService { get; set; }
        #endregion


        protected override async Task OnInitializedAsync()
        {
            var serverEmailConfig = await ServerEmailConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
            if (serverEmailConfig != null)
            {
                ServerEmailConfig = Mapper<ServerEmailConfigDQM, ServerEmailConfigDCM>.MapToNewObj(serverEmailConfig);
            }


            var serverShortMessageServiceConfig = await ServerShortMessageServiceConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
            if (serverShortMessageServiceConfig != null)
            {
                ServerShortMessageServiceConfig = Mapper<ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM>.MapToNewObj(serverShortMessageServiceConfig);
            }

            var systemAccountConfig = await SystemAccountConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
            if (systemAccountConfig != null)
            {
                SystemAccountConfig = Mapper<SystemAccountConfigDQM, SystemAccountConfigDCM>.MapToNewObj(systemAccountConfig);
            }
            else
            {
                await SystemAccountConfigService!.SetAndSaveEntityData(new SystemAccountConfigDCM() { Id = Guid.NewGuid(), TenantIdentifier = NowSystemTenant!.TenantIdentifier, CreateTime = DateTime.Now }, true);
                systemAccountConfig = await SystemAccountConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (systemAccountConfig != null)
                {
                    SystemAccountConfig = Mapper<SystemAccountConfigDQM, SystemAccountConfigDCM>.MapToNewObj(systemAccountConfig);
                }
            }


            var serverCommonConfig = await ServerCommonConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
            if (serverCommonConfig != null)
            {
                ServerCommonConfig = Mapper<ServerCommonConfigDQM, ServerCommonConfigDCM>.MapToNewObj(serverCommonConfig);
            }
            else
            {
                await ServerCommonConfigService!.SetAndSaveEntityData(new ServerCommonConfigDCM() { Id = Guid.NewGuid(), TenantIdentifier = NowSystemTenant!.TenantIdentifier, CreateTime = DateTime.Now }, true);
                serverCommonConfig = await ServerCommonConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (serverCommonConfig != null)
                {
                    ServerCommonConfig = Mapper<ServerCommonConfigDQM, ServerCommonConfigDCM>.MapToNewObj(serverCommonConfig);
                }
            }
        }


        private async Task InitServerEmailConfig()
        {
            var res = await ServerEmailConfigService!.SetAndSaveEntityData(new ServerEmailConfigDCM()
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                TenantIdentifier = NowSystemTenant!.TenantIdentifier
            });
            if (res.IsSuccess)
            {
                await MessageService!.Success(res.Message);
                var serverEmailConfig = await ServerEmailConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (serverEmailConfig != null)
                {
                    ServerEmailConfig = Mapper<ServerEmailConfigDQM, ServerEmailConfigDCM>.MapToNewObj(serverEmailConfig);
                }
                StateHasChanged();
            }
            else
            {
                await MessageService!.Error(res.Message);
            }
        }


        private async Task InitServerShortMessageServiceConfig()
        {
            var res = await ServerShortMessageServiceConfigService!.SetAndSaveEntityData(new ServerShortMessageServiceConfigDCM()
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                TenantIdentifier = NowSystemTenant!.TenantIdentifier
            });
            if (res.IsSuccess)
            {
                await MessageService!.Success(res.Message);
                var serverShortMessageServiceConfig = await ServerShortMessageServiceConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (serverShortMessageServiceConfig != null)
                {
                    ServerShortMessageServiceConfig = Mapper<ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM>.MapToNewObj(serverShortMessageServiceConfig);
                }
            }
            else
            {
                await MessageService!.Error(res.Message);
                StateHasChanged();
            }
        }

        private async Task UpdateServerCommonConfig()
        {
            var res = await ServerCommonConfigService!.SetAndSaveEntityData(ServerCommonConfig!);
            if (res.IsSuccess)
            {
                await MessageService!.Success(res.Message);
                var serverCommonConfig = await ServerCommonConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (serverCommonConfig != null)
                {
                    ServerCommonConfig = Mapper<ServerCommonConfigDQM, ServerCommonConfigDCM>.MapToNewObj(serverCommonConfig);
                }
                StateHasChanged();
            }
            else
            {
                await MessageService!.Error(res.Message);
            }
        }

        private async Task UpdateSystemAccountConfig()
        {
            var res = await SystemAccountConfigService!.SetAndSaveEntityData(SystemAccountConfig!);
            if (res.IsSuccess)
            {
                await MessageService!.Success(res.Message);
                var systemAccountConfig = await SystemAccountConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (systemAccountConfig != null)
                {
                    SystemAccountConfig = Mapper<SystemAccountConfigDQM, SystemAccountConfigDCM>.MapToNewObj(systemAccountConfig);
                }
                StateHasChanged();
            }
            else
            {
                await MessageService!.Error(res.Message);
            }
        }

        private async Task UpdateServerEmailConfig()
        {
            var res = await ServerEmailConfigService!.SetAndSaveEntityData(ServerEmailConfig!);
            if (res.IsSuccess)
            {
                await MessageService!.Success(res.Message);
                var serverEmailConfig = await ServerEmailConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (serverEmailConfig != null)
                {
                    ServerEmailConfig = Mapper<ServerEmailConfigDQM, ServerEmailConfigDCM>.MapToNewObj(serverEmailConfig);
                }
                StateHasChanged();
            }
            else
            {
                await MessageService!.Error(res.Message);
            }
        }


        private async Task UpdateServerShortMessageServiceConfig()
        {
            var res = await ServerShortMessageServiceConfigService!.SetAndSaveEntityData(ServerShortMessageServiceConfig!);
            if (res.IsSuccess)
            {
                await MessageService!.Success(res.Message);
                var serverShortMessageServiceConfig = await ServerShortMessageServiceConfigService!.GetApiBoAsync(x => x.TenantIdentifier == NowSystemTenant!.TenantIdentifier);
                if (serverShortMessageServiceConfig != null)
                {
                    ServerShortMessageServiceConfig = Mapper<ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM>.MapToNewObj(serverShortMessageServiceConfig);
                }
                StateHasChanged();
            }
            else
            {
                await MessageService!.Error(res.Message);
            }
        }


        #region Tools

        private string _AesPassword = "";

        private void CreateAesPassword(int byteNumber)
        {
            byte[] randomBytes = new byte[byteNumber];
            RandomNumberGenerator.Fill(randomBytes);
            _AesPassword = Convert.ToBase64String(randomBytes);
            StateHasChanged();
        }

        #endregion

    }
}

using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.ServerInfrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Page
{
    /// <summary>
    /// 定义一些常规使用的全局配置变量的基础视图
    /// </summary>
    public class GlobalConfigPageBase : ComponentBase
    {
        [Parameter]
        public SystemAccountConfigDQM? CurrentSystemAccountConfig { get; set; }
        [Parameter]
        public ServerCommonConfigDQM? CurrentServerCommonConfig { get; set; }
        [Parameter]
        public ServerEmailConfigDQM? CurrentServerEmailConfig { get; set; }
        [Parameter]
        public ServerShortMessageServiceConfigDQM? CurrentServerShortMessageServiceConfig { get; set; }
        [Inject]
        public GlobalSystemConfigService? GlobalSystemConfigService { get; set; }

        protected async Task LoadConfigsAsync()
        {
            CurrentSystemAccountConfig = await GlobalSystemConfigService!.GetSystemAccountConfig();
            CurrentServerCommonConfig = await GlobalSystemConfigService!.GetServerCommonConfig();
            CurrentServerEmailConfig = await GlobalSystemConfigService!.GetServerEmailConfig();
            CurrentServerShortMessageServiceConfig = await GlobalSystemConfigService!.GetServerShortMessageServiceConfig();
        }

        protected async Task LoadSystemAccountConfigAsync()
        {
            CurrentSystemAccountConfig = await GlobalSystemConfigService!.GetSystemAccountConfig();
        }

        protected async Task LoadServerCommonConfigAsync()
        {
            CurrentServerCommonConfig = await GlobalSystemConfigService!.GetServerCommonConfig();
        }

        protected async Task LoaServerEmailConfigAsync()
        {
            CurrentServerEmailConfig = await GlobalSystemConfigService!.GetServerEmailConfig();
        }

        protected async Task LoadServerShortMessageServiceConfigAsync()
        {
            CurrentServerShortMessageServiceConfig = await GlobalSystemConfigService!.GetServerShortMessageServiceConfig();
        }
    }
}

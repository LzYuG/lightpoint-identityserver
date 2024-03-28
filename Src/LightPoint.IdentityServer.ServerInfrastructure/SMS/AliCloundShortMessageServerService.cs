using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using LightPoint.IdentityServer.ServerInfrastructure.Email;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tea;

namespace LightPoint.IdentityServer.ServerInfrastructure.SMS
{
    public class AliCloundShortMessageServerService : IShortMessageServerService
    {
        private readonly ILogger<AliCloundShortMessageServerService> _logger;
        private readonly IAppService<Guid, ServerShortMessageServiceConfig, ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM> _service;
        private readonly IAppService<Guid, ServerRunningLog, ServerRunningLogDM, ServerRunningLogDM> _serverRunningLogService;
        private readonly IConfidentialService _confidentialService;

        public AliCloundShortMessageServerService(ILogger<AliCloundShortMessageServerService> logger,
            IAppService<Guid, ServerShortMessageServiceConfig, ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM> service,
            IAppService<Guid, ServerRunningLog, ServerRunningLogDM, ServerRunningLogDM> serverRunningLogService,
            IConfidentialService confidentialService)
        {
            _logger = logger;
            _service = service;
            _serverRunningLogService = serverRunningLogService;
            _confidentialService = confidentialService;
        }

        public async Task SendValidationCodeAsync(TenantInfo tenantInfo, ValidationCodeShortMessageModel model, CancellationToken cancellationToken)
        {
            var config = await _service.GetApiBoAsync(x => x.IsEnable && x.TenantIdentifier == tenantInfo.TenantIdentifier);
            if(config == null)
            {
                throw new InvalidOperationException("该租户未配置短信发送配置");
            }
            config = await _confidentialService.DecryptModelConfidentialPropsAsync(config);
            var client = CreateClient(config);

            var templateParam = config.ValidationCodeValueParam;
            templateParam = templateParam!.Replace("{CODE}", $"{model.ValidationCode}");

            AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest sendSmsRequest = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest
            {
                SignName = config.ValidationCodeSignName,
                TemplateCode = config.ValidationCodeTemplateId,
                PhoneNumbers = model.To,
                TemplateParam = templateParam,
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var response = client.SendSmsWithOptions(sendSmsRequest, runtime);
                if (response.Body.Code != "OK")
                {
                    await _serverRunningLogService.SetAndSaveEntityData(new ServerRunningLogDM()
                    {
                        CreateTime = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Remark = "阿里云短信发送错误",
                        ServerRunningLogType = Shared.BusinessEnums.BE04.LogAuditingResources.ServerRunningLogType.服务异常,
                        TenantIdentifier = tenantInfo.TenantIdentifier,
                        Errors = response.Body.Code,
                        RemoteIP = model.RemoteIP
                    }, true);
                }
            }
            catch (Exception error)
            {
                await _serverRunningLogService.SetAndSaveEntityData(new ServerRunningLogDM()
                {
                    CreateTime = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Remark = "阿里云短信发送错误",
                    ServerRunningLogType = Shared.BusinessEnums.BE04.LogAuditingResources.ServerRunningLogType.服务异常,
                    TenantIdentifier = tenantInfo.TenantIdentifier,
                    Errors = error.Message + "；\n" + error.Data["Recommend"] as string,
                    RemoteIP = model.RemoteIP
                }, true);
            }
        }

        private AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient(ServerShortMessageServiceConfigDQM smsConfig)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 必填，您的 AccessKey ID
                AccessKeyId = smsConfig.SMSAccount,
                // 必填，您的 AccessKey Secret
                AccessKeySecret = smsConfig.SMSPassword,
            };
            // Endpoint 请参考 https://api.aliyun.com/product/Dysmsapi
            // config.Endpoint = "dysmsapi.aliyuncs.com";
            config.Endpoint = smsConfig.SMSHost;
            return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
        }
    }
}

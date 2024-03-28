using AntDesign;
using Hangfire;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Shared;
using LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using LightPoint.IdentityServer.ServerInfrastructure.Email;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.ServerInfrastructure.SMS;
using LightPoint.IdentityServer.ServerInfrastructure.ValidationCode;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts
{
    partial class Register
    {
        [Inject]
        public IJSRuntime? JSRuntime { get; set; }
        [Inject]
        public TenantInfoAccessor? TenantInfoAccessor { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        [Inject]
        public IApplicationUserService? ApplicationUserService { get; set; }
        [Inject]
        public IIdentityServerClientService? IdentityServerClientService { get; set; }
        [Inject]
        public IMessageService? MessageService { get; set; }

        #region PageData
        private RegisterModel RegisterModel { get; set; } = new RegisterModel();


        private string? ClientId = "";

        private IdentityServerClientDQM? IdentityServerClient { get; set; }
        #endregion

        #region TemplateParams
        public IdentityServerBusinessTemplatePageParams IdentityServerBusinessTemplatePageParams { get; set; } = new IdentityServerBusinessTemplatePageParams();
        #endregion

        public List<string> Errors { get; set; } = new List<string>();
        // 默认模板
        public string? Template { get; set; } = "";
        [Inject]
        public IEmailService? EmailService { get; set; }
        [Inject]
        public IShortMessageServerService? ShortMessageServerService { get; set; }
        [Inject]
        public ILightPointCache? LightPointCache { get; set; }
        [Inject]
        public IValidationCodeCreater? ValidationCodeCreater { get; set; }
        [Inject]
        public IAppService<Guid, MessageSendedLog, MessageSendedLogDM, MessageSendedLogDM>? MessageSendedLogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadConfigsAsync();
            ClientId = NavigationManager!.GetQueryParam("client_id");

            IdentityServerClient = await IdentityServerClientService!.GetApiBoAsync(x => x.ClientId == ClientId && x.TenantIdentifier == TenantInfoAccessor!.TenantInfo!.TenantIdentifier, true);

            IdentityServerBusinessTemplatePageParams.SubTitle = $"正在使用：{PageConstants.MainApplicationName}";

            if (IdentityServerClient != null)
            {
                if (!string.IsNullOrWhiteSpace(IdentityServerClient.LoginPageBackgroundImgUri))
                {
                    IdentityServerBusinessTemplatePageParams.BackgroundImgUri = IdentityServerClient.LoginPageBackgroundImgUri;
                }
                if (!string.IsNullOrWhiteSpace(IdentityServerClient.LogoUri))
                {
                    IdentityServerBusinessTemplatePageParams.ClientLogoUri = IdentityServerClient.LogoUri;
                }
                if (!string.IsNullOrWhiteSpace(IdentityServerClient.PageTemplate))
                {
                    IdentityServerBusinessTemplatePageParams.Template = IdentityServerClient.PageTemplate;
                }
                if (!string.IsNullOrWhiteSpace(IdentityServerClient.Name))
                {
                    IdentityServerBusinessTemplatePageParams.ClientName = IdentityServerClient.Name;
                    IdentityServerBusinessTemplatePageParams.SubTitle = "正在使用：" + IdentityServerClient.Name;
                }
            }
        }

        private async Task Cancel()
        {
            await JSRuntime!.InvokeVoidAsync("history.back");
        }


        private Task RegisterSubmit()
        {
            throw new NotImplementedException();
        }

        #region 发送验证码辅助方法

        /// <summary>
        /// 邮件发送间隔
        /// </summary>
        public int ShortMessageSendingIterval { get; set; }
        public Timer? ShortMessageSSendingItervalTimer { get; set; }
        public int EmailSendingIterval { get; set; }
        public Timer? EmailSendingItervalTimer { get; set; }

        public async Task SendResetPasswordByPhoneNumberValidationCodeShortMessage()
        {
            if (string.IsNullOrWhiteSpace(RegisterModel.PhoneNumber))
            {
                await MessageService!.Error("请先输入手机号码");
                return;
            }
            if (!await ApplicationUserService!.HasBoAsync(x => x.PhoneNumber == RegisterModel.PhoneNumber! && x.PhoneNumberConfirmed))
            {
                await MessageService!.Error("手机号码还未注册账号");
                return;
            }

            await SendValidationCodeShortMessage("RegisterValidatePhone", "用于该用户忘记密码时进行重置", RegisterModel.PhoneNumber!);
        }

        private async Task SendValidationCodeShortMessage(string operationType, string remark, string phoneNumber)
        {
            var pattern = this.CurrentServerShortMessageServiceConfig!.PhoneNumberValidationRegex!;
            var regex = new Regex(pattern);

            if (phoneNumber == null || !regex.IsMatch(phoneNumber))
            {
                await MessageService!.Error("手机号码格式错误");
                return;
            }

            ShortMessageSendingIterval = 60;
            ShortMessageSSendingItervalTimer = new Timer((object? state) => { ShortMessageSendingIterval -= 1; InvokeAsync(StateHasChanged); if (ShortMessageSendingIterval == 0) ShortMessageSSendingItervalTimer?.Dispose(); }, null, 0, 1000);

            var toadySendedShortMessageCountWithSameIPAndPhone = await MessageSendedLogService!.GetApiAmountAsync(x => x.CreateTime.Date == DateTime.Now.Date && x.MessageSendedLogType == IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.短信验证码
                && x.RemoteIP == TenantInfoAccessor!.CurrentRemoteIP && x.To == phoneNumber);

            if (toadySendedShortMessageCountWithSameIPAndPhone >= this.CurrentServerShortMessageServiceConfig!.DailySendLimitWithSameIPAndPhone)
            {
                await MessageService!.Error("超出每日短信发送限制");
                return;
            }

            var toadySendedShortMessageCountWithSameIP = await MessageSendedLogService!.GetApiAmountAsync(x => x.CreateTime.Date == DateTime.Now.Date && x.MessageSendedLogType == IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.短信验证码
                && x.RemoteIP == TenantInfoAccessor!.CurrentRemoteIP);

            if (toadySendedShortMessageCountWithSameIP >= this.CurrentServerShortMessageServiceConfig!.DailySendLimitWithSameIP)
            {
                await MessageService!.Error("超出每日短信发送限制");
                return;
            }

            var cacheModel = await LightPointCache!.GetItemAsymc<ValidationCodeModel>(ValidationCodeCreater!.CreateValidationCodeCacheKey(operationType, phoneNumber));

            if (cacheModel != null && cacheModel!.CreateTime.AddMinutes(1) > DateTime.Now)
            {
                await MessageService!.Error("发送过于频繁，请稍后重试");
                ShortMessageSendingIterval = (int)(DateTime.Now - cacheModel!.CreateTime).TotalSeconds;
                StateHasChanged();
                return;
            }

            var ctk = new CancellationToken();
            var codeModel = await ValidationCodeCreater!.CreateValidationCode(operationType, phoneNumber, 6);
            BackgroundJob.Enqueue(() => ShortMessageServerService!.SendValidationCodeAsync(TenantInfoAccessor!.TenantInfo,
                new ValidationCodeShortMessageModel()
                {
                    RemoteIP = TenantInfoAccessor.CurrentRemoteIP,
                    To = phoneNumber,
                    ValidationCode = codeModel.Code
                },
                ctk));
            var content = this.CurrentServerShortMessageServiceConfig.PlatformType + "：短信验证码";
            if (!string.IsNullOrWhiteSpace(this.CurrentServerShortMessageServiceConfig.ValidationCodeTemplate))
            {
                content = this.CurrentServerShortMessageServiceConfig.PlatformType + "：" + this.CurrentServerShortMessageServiceConfig.ValidationCodeTemplate.Replace("{CODE}", codeModel.Code);
            }

            await MessageService!.Success("短信验证码已发送至该手机号码");
            await MessageSendedLogService.SetAndSaveEntityData(new MessageSendedLogDM()
            {
                CreateTime = DateTime.Now,
                Id = Guid.NewGuid(),
                KeyValue1 = codeModel.Code,
                Remark = remark,
                MessageContentDescription = content,
                RemoteIP = TenantInfoAccessor!.CurrentRemoteIP,
                To = phoneNumber,
                OperationIdentityId = phoneNumber,
                OperationIdentityName = "登录手机号码：" + phoneNumber,
                TenantIdentifier = TenantInfoAccessor.TenantInfo.TenantIdentifier,
                MessageSendedLogType = IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.短信验证码,
            });
        }

        public async Task SendResetPasswordByEmailValidationCodeEmail()
        {
            if (string.IsNullOrWhiteSpace(RegisterModel.Email))
            {
                await MessageService!.Error("请先输入邮箱号码");
                return;
            }
            if (!await ApplicationUserService!.HasBoAsync(x => x.Email == RegisterModel.Email && x.EmailConfirmed))
            {
                await MessageService!.Error("邮箱还未注册账号");
                return;
            }
            await SendValidationCodeEmail("RegisterValidateEmail", $"您正在注册新用户", "用于该用户忘记密码时进行重置", RegisterModel.Email!);
        }

        private async Task SendValidationCodeEmail(string operationType, string operationTypeInEmail, string remark, string email)
        {
            var pattern = this.CurrentServerEmailConfig!.EmailValidationRegex!;
            var regex = new Regex(pattern);

            if (email == null || !regex.IsMatch(email))
            {
                await MessageService!.Error("邮箱格式错误");
                return;
            }

            EmailSendingIterval = 60;
            EmailSendingItervalTimer = new Timer((object? state) => { EmailSendingIterval -= 1; InvokeAsync(StateHasChanged); if (EmailSendingIterval == 0) EmailSendingItervalTimer?.Dispose(); }, null, 0, 1000);

            var toadySendedEmailCountWithSameIPAndEmail = await MessageSendedLogService!.GetApiAmountAsync(x => x.CreateTime.Date == DateTime.Now.Date && x.MessageSendedLogType == IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.验证码邮件
                && x.RemoteIP == TenantInfoAccessor!.CurrentRemoteIP && x.To == email);

            if (toadySendedEmailCountWithSameIPAndEmail >= this.CurrentServerEmailConfig!.DailySendLimitWithSameIPAndEmail)
            {
                await MessageService!.Error("超出每日邮件发送限制");
                return;
            }

            var toadySendedEmailCountWithSameIP = await MessageSendedLogService!.GetApiAmountAsync(x => x.CreateTime.Date == DateTime.Now.Date && x.MessageSendedLogType == IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.验证码邮件
                && x.RemoteIP == TenantInfoAccessor!.CurrentRemoteIP);

            if (toadySendedEmailCountWithSameIP >= this.CurrentServerEmailConfig!.DailySendLimitWithSameIP)
            {
                await MessageService!.Error("超出每日邮件发送限制");
                return;
            }

            var cacheModel = await LightPointCache!.GetItemAsymc<ValidationCodeModel>(ValidationCodeCreater!.CreateValidationCodeCacheKey(operationType, email));

            if (cacheModel != null && cacheModel!.CreateTime.AddMinutes(1) > DateTime.Now)
            {
                await MessageService!.Error("发送过于频繁，请稍后重试");
                EmailSendingIterval = (int)(DateTime.Now - cacheModel!.CreateTime).TotalSeconds;
                StateHasChanged();
                return;
            }

            var ctk = new CancellationToken();
            var codeModel = await ValidationCodeCreater!.CreateValidationCode(operationType, email, 6);
            BackgroundJob.Enqueue(() => EmailService!.SendValidationCodeAsync(TenantInfoAccessor!.TenantInfo,
                new ValidationCodeEmailModel(new List<string>() { email })
                {
                    ClientName = "身份认证服务",
                    Code = codeModel.Code,
                    RemoteIP = TenantInfoAccessor.CurrentRemoteIP,
                    OpertionType = operationTypeInEmail,
                },
                ctk));
            await MessageService!.Success("邮件已发送至该邮箱");
            await MessageSendedLogService.SetAndSaveEntityData(new MessageSendedLogDM()
            {
                CreateTime = DateTime.Now,
                Id = Guid.NewGuid(),
                Remark = remark,
                KeyValue1 = codeModel.Code,
                MessageContentDescription = "邮箱发送的验证码",
                RemoteIP = TenantInfoAccessor!.CurrentRemoteIP,
                To = email,
                OperationIdentityId = email,
                OperationIdentityName = "登录邮箱：" + email,
                TenantIdentifier = TenantInfoAccessor.TenantInfo.TenantIdentifier,
                MessageSendedLogType = IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.验证码邮件,
            });
        }
        #endregion
    }
}

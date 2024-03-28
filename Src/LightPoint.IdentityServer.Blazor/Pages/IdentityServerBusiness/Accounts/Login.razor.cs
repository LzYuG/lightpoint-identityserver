using AntDesign;
using Hangfire;
using LightPoint.IdentityServer.Blazor.Components.Tools;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Shared;
using LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.Models;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using LightPoint.IdentityServer.ServerInfrastructure.Email;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.ServerInfrastructure.SMS;
using LightPoint.IdentityServer.ServerInfrastructure.ValidationCode;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources;
using LightPoint.IdentityServer.Shared.Helpers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LightPoint.IdentityServer.Shared.Helpers.OidcConstants;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts
{
    partial class Login : GlobalConfigPageBase
    {
        [Parameter]
        public LoginViewModel? LoginViewModel { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        [Inject]
        public IIdentityServerAccountPageService? IdentityServerAccountPageService { get; set; }
        [Inject]
        public IApplicationUserService? ApplicationUserService { get; set; }
        [Inject]
        public IIdentityServerClientService? IdentityServerClientService { get; set; }
        [Inject]
        public TenantInfoAccessor? TenantInfoAccessor { get; set; }
        [Inject]
        public IMessageService? MessageService { get; set; }
        [Inject]
        public ToolsJsInterop? ToolsJsInterop { get; set; }
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
        [Inject]
        public IAppService<Guid, ApplicationUserLoginedLog, ApplicationUserLoginedLogDM, ApplicationUserLoginedLogDM>? ApplicationUserLoginedLogService { get; set; }
        [Parameter]
        public string? AntiForgeryToken { get; set; }

        #region PageData
        private LoginInputModel LoginInputModel { get; set; } = new LoginInputModel();

        private string TabActiveKey = "0";

        private string? ClientId { get; set; }

        private IdentityServerClientDQM? IdentityServerClient { get; set; }
        #endregion

        #region TemplateParams
        public IdentityServerBusinessTemplatePageParams IdentityServerBusinessTemplatePageParams { get; set; } = new IdentityServerBusinessTemplatePageParams();
        #endregion

        // 默认模板
        public string? Template { get; set; } = "";

        public bool InitComplated { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var returnUrl = NavigationManager!.GetQueryParam("returnUrl");
            LoginInputModel.ReturnUrl = returnUrl;
            IdentityServerBusinessTemplatePageParams.SubTitle = $"正在使用：{PageConstants.MainApplicationName}";

            if (!string.IsNullOrWhiteSpace(LoginInputModel.ReturnUrl) && LoginInputModel.ReturnUrl.StartsWith("/connect/authorize/callback?"))
            {
                var callbackQuery = LoginInputModel.ReturnUrl.Split("?")[1];
                var clientiParams = callbackQuery.Split("&").FirstOrDefault(x => x.StartsWith("client_id="));
                if(clientiParams != null && clientiParams.Split("=").Length == 2)
                {
                    ClientId = clientiParams.Split("=")[1];
                }

                IdentityServerClient = await IdentityServerClientService!.GetApiBoAsync(x => x.ClientId == ClientId && x.TenantIdentifier == TenantInfoAccessor!.TenantInfo!.TenantIdentifier, true);

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
            await LoadConfigsAsync();
            InitComplated = true;
            StateHasChanged();
        }

        private async Task GoForgetPassword()
        {
            await ToolsJsInterop!.Redirect("/Account/ForgetPassword?client_id=" + ClientId);
        }

        private async Task GoIdentityServerLogin()
        {
            await ToolsJsInterop!.Redirect("/Account/Login?returnUrl=");
        }

        private async Task GoRegister()
        {
            await ToolsJsInterop!.Redirect("/Account/Register?client_id=" + ClientId);
        }

        /// <summary>
        /// 登录提交
        /// 由于需要导航至MVC，避免反复跳转的损耗，这里也进行登录凭据的校验，注意：RazorPage也需要对表单再进行一次校验
        /// </summary>
        /// <param name="model"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task LoginSubmit()
        {
            LoginInputModel.LoginType = (LoginType)int.Parse(TabActiveKey);

            if (LoginInputModel.LoginType == LoginType.账号密码)
            {
                // 账号密码登录
                if (string.IsNullOrWhiteSpace(LoginInputModel.Username) || string.IsNullOrWhiteSpace(LoginInputModel.Password))
                {
                    await MessageService!.Error("账号或密码错误");
                }
                else
                {
                    if (await ValidateUserIsLock(x => x.UserName == LoginInputModel.Username))
                    {
                        return;
                    }
                    if ((await ApplicationUserService!.ValidateCredentials(LoginInputModel.Username!, LoginInputModel.Password!)).IsSuccess)
                    {
                        if (ApplicationUser!.TwoFactorEnabled && ApplicationUser.MutilFactorEnabled && ApplicationUser.ApplicationUserMultiFactorAuthentications!.Any(x => x.IsEnable))
                        {
                            IsInMFAValidationState = true;
                            await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.账号密码登录, true, LoginInputModel.Username!, LoginInputModel.Password!, "登录成功，进入MFA校验");
                            return;
                        }
                        await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.账号密码登录, true, LoginInputModel.Username!, LoginInputModel.Password!, "登录成功");
                        // 使用表单请求上传至 /Account/Login Post
                        await ToolsJsInterop!.PostForm("/Account/Login", LoginInputModel, AntiForgeryToken);
                    }
                    else
                    {
                        await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.账号密码登录, false, LoginInputModel.Username!, LoginInputModel.Password!, "账号或密码错误");
                        await MessageService!.Error("账号或密码错误");
                    }
                }
            }
            else if (LoginInputModel.LoginType == LoginType.短信验证码)
            {
                if (string.IsNullOrWhiteSpace(LoginInputModel.PhoneNumber) || string.IsNullOrWhiteSpace(LoginInputModel.ShortMessageCode))
                {
                    await MessageService!.Error("短信验证码校验失败");
                }
                // 短信验证码登录
                else
                {
                    if (await ValidateUserIsLock(x => x.PhoneNumber == LoginInputModel.PhoneNumber && x.PhoneNumberConfirmed))
                    {
                        return;
                    }

                    var validateResult = await ValidationCodeCreater!.ValidateValidationCode("PhoneNumberLogin", LoginInputModel.PhoneNumber!, LoginInputModel.ShortMessageCode!, false);
                    if (validateResult)
                    {
                        await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.手机验证码登录, true, LoginInputModel.PhoneNumber!, LoginInputModel.ShortMessageCode!, "登录成功");
                        // 使用表单请求上传至 /Account/Login Post
                        await ToolsJsInterop!.PostForm("/Account/Login", LoginInputModel, AntiForgeryToken);
                    }
                    else
                    {
                        await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.手机验证码登录, false, LoginInputModel.PhoneNumber!, LoginInputModel.ShortMessageCode!, "短信验证码校验失败");
                        await MessageService!.Error("短信验证码校验失败");
                    }
                }
            }
            else if (LoginInputModel.LoginType == LoginType.邮箱验证码)
            {
                if (string.IsNullOrWhiteSpace(LoginInputModel.Email) || string.IsNullOrWhiteSpace(LoginInputModel.EmailCode))
                {
                    await MessageService!.Error("邮箱验证码校验失败");
                }
                // 短信验证码登录
                else
                {
                    if(await ValidateUserIsLock(x => x.Email == LoginInputModel.Email && x.EmailConfirmed))
                    {
                        return;
                    }

                    var validateResult = await ValidationCodeCreater!.ValidateValidationCode("EmailLogin", LoginInputModel.Email!, LoginInputModel.EmailCode!, false);
                    if (validateResult)
                    {
                        await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.邮箱验证码登录, true, LoginInputModel.Email!, LoginInputModel.EmailCode!, "登录成功");
                        // 使用表单请求上传至 /Account/Login Post
                        await ToolsJsInterop!.PostForm("/Account/Login", LoginInputModel, AntiForgeryToken);
                    }
                    else
                    {
                        await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.邮箱验证码登录, false, LoginInputModel.Email!, LoginInputModel.EmailCode!, "邮箱验证码校验失败");
                        await MessageService!.Error("邮箱验证码校验失败");
                    }
                }
            }

            StateHasChanged();
        }


        private async Task<bool> ValidateUserIsLock(Expression<Func<ApplicationUser, bool>> expression)
        {
            ApplicationUser = await ApplicationUserService!.GetApiBoAsync(expression, true);
            if (ApplicationUser == null)
            {
                await MessageService!.Error("短信验证码校验失败");
                return true;
            }
            if (await _ValidateUserIsLock(ApplicationUser.Id))
            {
                await MessageService!.Error("账号登录失败次数过多，已被锁定，请五分钟后重试");
                return true;
            }

            return false;
        }

        private async Task<bool> _ValidateUserIsLock(Guid userId)
        {
            var recentlyErrorAmount = await ApplicationUserLoginedLogService!.GetApiAmountAsync(x => x.RemoteIP == this.TenantInfoAccessor!.CurrentRemoteIP && !x.IsSuccess && x.ApplicationUserId == userId
                && x.CreateTime >= DateTime.Now.AddMinutes(-5) && x.CreateTime <= DateTime.Now);

            if(recentlyErrorAmount >= 5)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private async Task AddUserLoginLog(Guid userId, string userName, ApplicationUserLoginedLogType type, bool isSuccess, string inputName, string inputPassword, string loginResultDescription)
        {
            if(type == ApplicationUserLoginedLogType.账号密码登录)
            {
                inputPassword = "*****";
            }
            await ApplicationUserLoginedLogService!.SetAndSaveEntityData(new ApplicationUserLoginedLogDM()
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = userId,
                ApplicationUserName = userName,
                ApplicationUserLoginedLogType = type,
                CreateTime = DateTime.Now,
                InputPassword = inputPassword,
                InputUserName = inputName,
                IsSuccess = isSuccess,
                LoginResultDescription = loginResultDescription,
                RemoteIP = this.TenantInfoAccessor!.CurrentRemoteIP,
                TenantIdentifier = this.TenantInfoAccessor.TenantInfo.TenantIdentifier,
            });
        }

        #region 发送验证码辅助方法

        /// <summary>
        /// 邮件发送间隔
        /// </summary>
        public int ShortMessageSendingIterval { get; set; }
        public Timer? ShortMessageSSendingItervalTimer { get; set; }
        public int EmailSendingIterval { get; set; }
        public Timer? EmailSendingItervalTimer { get; set; }

        public async Task SendLoginByPhoneNumberValidationCodeShortMessage()
        {
            if (string.IsNullOrWhiteSpace(LoginInputModel.PhoneNumber))
            {
                await MessageService!.Error("请先输入手机号码");
                return;
            }
            if (!await ApplicationUserService!.HasBoAsync(x => x.PhoneNumber == LoginInputModel.PhoneNumber! && x.PhoneNumberConfirmed))
            {
                await MessageService!.Error("手机号码还未注册账号");
                return;
            }

            await SendValidationCodeShortMessage("PhoneNumberLogin", "用于该用户进行登录", LoginInputModel.PhoneNumber!);
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

        public async Task SendLoginByEmailValidationCodeEmail()
        {
            if (string.IsNullOrWhiteSpace(LoginInputModel.Email))
            {
                await MessageService!.Error("请先输入邮箱号码");
                return;
            }
            if (!await ApplicationUserService!.HasBoAsync(x => x.Email == LoginInputModel.Email && x.EmailConfirmed))
            {
                await MessageService!.Error("邮箱还未注册账号");
                return;
            }
            await SendValidationCodeEmail("EmailLogin", $"您正在进行登录操作", "用于该用户进行登录", LoginInputModel.Email!);
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

        #region MFA
        /// <summary>
        /// 是否进入MFA校验状态
        /// </summary>
        private bool IsInMFAValidationState { get; set; }
        /// <summary>
        /// 账号密码登录的情况下，先获取要登陆的用户
        /// </summary>
        public ApplicationUserDQM? ApplicationUser { get; set; }

        private async Task MFASuccessRedirect(Tuple<string, ApplicationUserMultiFactorAuthenticationType> data)
        {
            LoginInputModel.MFAToken = data.Item1;
            LoginInputModel.TowFactorAuthenticationType = data.Item2;

            await AddUserLoginLog(ApplicationUser!.Id, ApplicationUser.UserName!, ApplicationUserLoginedLogType.账号密码登录, true, LoginInputModel.Username!, LoginInputModel.Password!, "MFA校验成功，登录成功");
            // 使用表单请求上传至 /Account/Login Post
            await ToolsJsInterop!.PostForm("/Account/Login", LoginInputModel, AntiForgeryToken);
        }
        #endregion
    }
}

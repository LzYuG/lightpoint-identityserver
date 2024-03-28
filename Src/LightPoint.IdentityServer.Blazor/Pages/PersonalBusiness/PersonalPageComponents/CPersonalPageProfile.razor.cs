using AntDesign;
using Hangfire;
using LightPoint.IdentityServer.Blazor.Layout.Helpers;
using LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.Models;
using LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using LightPoint.IdentityServer.ServerInfrastructure.Email;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.ServerInfrastructure.SMS;
using LightPoint.IdentityServer.ServerInfrastructure.ValidationCode;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.PersonalPageComponents
{
    partial class CPersonalPageProfile
    {
        [Inject]
        public IApplicationUserService? ApplicationUserService { get; set; }
        [Inject]
        public TenantInfoAccessor? TenantInfoAccessor { get; set; }
        [Inject]
        public IMessageService? MessageService { get; set; }
        [Inject]
        public IEmailService? EmailService { get; set; }
        [Inject]
        public IShortMessageServerService? ShortMessageServerService { get; set; }
        [Inject]
        public ILightPointCache? LightPointCache { get; set; }
        [Inject]
        public IValidationCodeCreater? ValidationCodeCreater { get; set; }
        [Inject]
        public ILayoutInitor? LayoutInitor { get; set; }

        [Inject]
        public IAppService<Guid, MessageSendedLog, MessageSendedLogDM, MessageSendedLogDM>? MessageSendedLogService { get; set; }

        internal PhoneNumberConfirmModel PhoneNumberConfirmModel { get; set; } = new PhoneNumberConfirmModel();
        internal EmailConfirmModel EmailConfirmModel { get; set; } = new EmailConfirmModel();
        [Parameter]
        public ApplicationUserDCM? ApplicationUserDCM { get; set; }
        [Parameter]
        public EventCallback UpdatedApplicationUser { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadUser();
                StateHasChanged();
            }
        }

        private async Task LoadUser()
        {
            await UpdatedApplicationUser.InvokeAsync();

            PhoneNumberConfirmModel!.PhoneNumber = ApplicationUserDCM!.PhoneNumber;
            PhoneNumberConfirmModel!.Confirmed = ApplicationUserDCM!.PhoneNumberConfirmed;

            EmailConfirmModel!.Email = ApplicationUserDCM!.Email;
            EmailConfirmModel!.Confirmed = ApplicationUserDCM!.EmailConfirmed;
        }

        private async Task SubmitForm()
        {
            var res = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM!, true);
            if (res.IsSuccess)
            {
                await UpdatedApplicationUser.InvokeAsync();
                await MessageService!.Success(res.Message);
            }
            else
            {
                await MessageService!.Error(res.Message);
            }
        }


        #region 手机短信验证
        /// <summary>
        /// 邮件发送间隔
        /// </summary>
        public int ShortMessageSendingIterval { get; set; }
        public Timer? ShortMessageSSendingItervalTimer { get; set; }

        
        public async Task SendConfirmPhoneNumberValidationCodeShortMessage()
        {
            if (await ApplicationUserService!.HasBoAsync(x => x.PhoneNumber == PhoneNumberConfirmModel.PhoneNumber! && x.PhoneNumberConfirmed))
            {
                await MessageService!.Error("手机号码已被其他账号绑定");
                return;
            }

            await SendValidationCodeShortMessage("PhoneNumberConfirm", "用于该用户进行邮箱绑定", PhoneNumberConfirmModel.PhoneNumber!);
        }

        private async Task ConfirmPhoneNumberSubmit()
        {
            var validateResult = await ValidationCodeCreater!.ValidateValidationCode("PhoneNumberConfirm", PhoneNumberConfirmModel.PhoneNumber!, PhoneNumberConfirmModel.ValidateCode!);
            if (validateResult)
            {
                ApplicationUserDCM!.PhoneNumber = PhoneNumberConfirmModel.PhoneNumber!;
                ApplicationUserDCM!.PhoneNumberConfirmed = true;
                var res = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM!, true);

                if (res.IsSuccess)
                {
                    await UpdatedApplicationUser.InvokeAsync();
                    PhoneNumberConfirmModel.PhoneNumber = PhoneNumberConfirmModel.PhoneNumber!;
                    PhoneNumberConfirmModel!.Confirmed = true;
                    await MessageService!.Success(res.Message);
                }
                else
                {
                    await MessageService!.Error(res.Message);
                }
                await LoadUser();
                StateHasChanged();
            }
            else
            {
                await MessageService!.Error("验证码错误");
                return;
            }

        }
        #endregion


        #region 邮箱验证
        /// <summary>
        /// 邮件发送间隔
        /// </summary>
        public int EmailSendingIterval { get; set; }
        public Timer? EmailSendingItervalTimer { get; set; }

        
        public async Task SendConfirmEmailValidationCodeEmail()
        {
            if (await ApplicationUserService!.HasBoAsync(x => x.Email == EmailConfirmModel.Email && x.EmailConfirmed))
            {
                await MessageService!.Error("邮箱已被其他账号绑定");
                return;
            }
            await SendValidationCodeEmail("EmailConfirm", $"您正在进行邮箱绑定操作（账号：{ApplicationUserDCM!.UserName}）", "用于该用户进行邮箱绑定", EmailConfirmModel.Email!);
        }

        private async Task ConfirmEmailSubmit()
        {
            var validateResult = await ValidationCodeCreater!.ValidateValidationCode("EmailConfirm", EmailConfirmModel.Email!, EmailConfirmModel.ValidateCode!);
            if (validateResult)
            {
                ApplicationUserDCM!.Email = EmailConfirmModel.Email!;
                ApplicationUserDCM!.EmailConfirmed = true;

                var res = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM!, true);

                if (res.IsSuccess)
                {
                    await UpdatedApplicationUser.InvokeAsync();
                    EmailConfirmModel.Email = EmailConfirmModel.Email!;
                    EmailConfirmModel!.Confirmed = true;
                    await MessageService!.Success(res.Message);
                }
                else
                {
                    await MessageService!.Error(res.Message);
                }
                await LoadUser();
                StateHasChanged();

            }
            else
            {
                await MessageService!.Error("验证码错误");
                return;
            }

        }


        #endregion

        #region 重置密码
        /*************************************** 使用旧密码 *****************************************************/
        public string? OldPassword { get; set; } = "";

        public string? NewPassword { get; set; } = "";

        public string? ConfirmPassword { get; set; } = "";

        private async Task SubmitResetPasswordByOldPassword()
        {
            if(!(await ApplicationUserService!.ValidateCredentials(this.ApplicationUserDCM!.UserName!, OldPassword!)).IsSuccess)
            {
                await MessageService!.Error("旧密码错误");
                return;
            }

            if(NewPassword!.Trim() != ConfirmPassword!.Trim())
            {
                await MessageService!.Error("两次密码输入不一致");
                return;
            }

            var resetPasswordRes = await ApplicationUserService.ResetPassword(CurrentSystemAccountConfig, ApplicationUserDCM.UserName!, NewPassword);

            if(resetPasswordRes.IsSuccess)
            {
                await UpdatedApplicationUser.InvokeAsync();
                await MessageService!.Success(resetPasswordRes.Message);
                await LayoutInitor!.Logout();
            }
            else
            {
                await MessageService!.Error(resetPasswordRes.Message);
            }
        }

        /*************************************** 使用手机号码 *****************************************************/

        public string? ResetPasswordShortMessageValidationCode { get; set; }

        private async Task SendResetPasswordShortMessageValidationCode()
        {
            if(this.ApplicationUserDCM!.PhoneNumber == null || !this.ApplicationUserDCM.PhoneNumberConfirmed)
            {
                await MessageService!.Error("手机号码还未绑定成功");
                return;
            }
            await SendValidationCodeShortMessage("ResetPasswordByShortMessageValidationCode", "用于该用户进行密码修改", ApplicationUserDCM.PhoneNumber);
        }

        private async Task SubmitResetPasswordByShortMessageValidationCode()
        {
            if (NewPassword!.Trim() != ConfirmPassword!.Trim())
            {
                await MessageService!.Error("两次密码输入不一致");
                return;
            }

            if(ResetPasswordShortMessageValidationCode == null)
            {
                await MessageService!.Error("校验码错误");
                return;
            }

            var validateResult = await ValidationCodeCreater!.ValidateValidationCode("ResetPasswordByShortMessageValidationCode", ApplicationUserDCM!.PhoneNumber!, ResetPasswordShortMessageValidationCode!);

            if (validateResult)
            {
                var resetPasswordRes = await ApplicationUserService!.ResetPassword(CurrentSystemAccountConfig, ApplicationUserDCM!.UserName!, NewPassword);

                if (resetPasswordRes.IsSuccess)
                {
                    await UpdatedApplicationUser.InvokeAsync();
                    await MessageService!.Success(resetPasswordRes.Message);
                    await LayoutInitor!.Logout();
                }
                else
                {
                    await MessageService!.Error(resetPasswordRes.Message);
                }
            }
            else
            {
                await MessageService!.Error("验证码错误");
                return;
            }
        }



        /*************************************** 使用邮箱 *****************************************************/

        public string? ResetPasswordEmailValidationCode { get; set; }

        private async Task SendResetPasswordEmailValidationCode()
        {
            if (this.ApplicationUserDCM!.Email == null || !this.ApplicationUserDCM.EmailConfirmed)
            {
                await MessageService!.Error("邮箱还未绑定成功");
                return;
            }
            await SendValidationCodeEmail("ResetPasswordByEmailValidationCode", $"您正在进行修改密码操作（账号：{ApplicationUserDCM!.UserName}）", "用于该用户进行密码修改", this.ApplicationUserDCM!.Email);
        }

        private async Task SubmitResetPasswordByEmailValidationCode()
        {
            if (NewPassword!.Trim() != ConfirmPassword!.Trim())
            {
                await MessageService!.Error("两次密码输入不一致");
                return;
            }

            if (ResetPasswordEmailValidationCode == null)
            {
                await MessageService!.Error("校验码错误");
                return;
            }

            var validateResult = await ValidationCodeCreater!.ValidateValidationCode("ResetPasswordByEmailValidationCode", ApplicationUserDCM!.Email!, ResetPasswordEmailValidationCode);

            if (validateResult)
            {
                var resetPasswordRes = await ApplicationUserService!.ResetPassword(CurrentSystemAccountConfig, ApplicationUserDCM!.UserName!, NewPassword);

                if (resetPasswordRes.IsSuccess)
                {
                    await UpdatedApplicationUser.InvokeAsync();
                    await MessageService!.Success(resetPasswordRes.Message);
                    await LayoutInitor!.Logout();
                }
                else
                {
                    await MessageService!.Error(resetPasswordRes.Message);
                }
            }
            else
            {
                await MessageService!.Error("验证码错误");
                return;
            }
        }

        #endregion


        #region 发送验证码辅助方法

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
                OperationIdentityName = ApplicationUserDCM!.UserName,
                TenantIdentifier = TenantInfoAccessor.TenantInfo.TenantIdentifier,
                MessageSendedLogType = IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.短信验证码,
            });
            await MessageService!.Success("短信验证码已发送至该手机号码");
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
                OperationIdentityName = ApplicationUserDCM!.UserName,
                TenantIdentifier = TenantInfoAccessor.TenantInfo.TenantIdentifier,
                MessageSendedLogType = IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources.MessageSendedLogType.验证码邮件,
            });
            await MessageService!.Success("邮件已发送至该邮箱");
        }
        #endregion

        public void Dispose()
        {
            EmailSendingItervalTimer?.Dispose();
            ShortMessageSSendingItervalTimer?.Dispose();
        }

    }
}

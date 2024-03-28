using AntDesign;
using AntDesign.TableModels;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Security.MFA.GoogleAuthenticator;
using LightPoint.IdentityServer.ServerInfrastructure.Tools.QRCode;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using Microsoft.AspNetCore.Components.Authorization;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.PersonalPageComponents
{
    /// <summary>
    /// MFA绑定
    /// </summary>
    partial class CPersonalPageMFABinding : GlobalConfigPageBase
    {
        [Parameter]
        public ApplicationUserDCM? ApplicationUserDCM { get; set; }
        [Parameter]
        public EventCallback UpdatedApplicationUser { get; set; }
        [Inject]
        public IMessageService? MessageService { get; set; }
        [Inject]
        public ModalService? ModalService { get; set; }
        [Inject]
        public IApplicationUserService? ApplicationUserService { get; set; }

        public GoogleAuthenticator? GoogleAuthenticator { get; set; }

        public SetupCode? SetupCode { get; set; }

        public string? InputTOTPCode { get; set; }

        private const string AndroidAuthenticatorDownloadUri = "https://jms-pkg.oss-cn-beijing.aliyuncs.com/Google%20Authenticator_v5.10_apkpure.com.apk";
        private string? AndroidAuthenticatorDownloadUriQRCode { get; set; } = QRCodeCreater.CreateQRCodeImage(AndroidAuthenticatorDownloadUri, 200);

        private const string IOSAuthenticatorDownloadUri = "https://apps.apple.com/app/microsoft-authenticator/id983156458";
        private string? IOSAuthenticatorDownloadUriQRCode { get; set; } = QRCodeCreater.CreateQRCodeImage(IOSAuthenticatorDownloadUri, 200);

        private string? TOTPSecret = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GoogleAuthenticator = new GoogleAuthenticator();
                await LoadUser();
                TOTPSecret = Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");
                SetupCode = GoogleAuthenticator.GenerateSetupCode("LightPointIdentityServer", ApplicationUserDCM!.UserName!, TOTPSecret, 200);
                StateHasChanged();
            }
        }

        private async Task AddTOTPAuthenticationSubmit()
        {
            if (GoogleAuthenticator!.ValidateTwoFactorPIN(TOTPSecret!, InputTOTPCode!))
            {
                var newData = new ApplicationUserMultiFactorAuthenticationDM()
                {
                    ApplicationUserId = ApplicationUserDCM!.Id,
                    ApplicationUserMultiFactorAuthenticationType = ApplicationUserMultiFactorAuthenticationType.时间型动态令牌,
                    IsEnable = true,
                    FactorValue = TOTPSecret,
                    Secret = MFASecretCreate(),
                    CreateTime = DateTime.Now,
                };
                ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.Add(newData);
                StateHasChanged();
                var result = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM, true);
                if (result.IsSuccess)
                {
                    await UpdatedApplicationUser.InvokeAsync();
                    await MessageService!.Success(result.Message);
                }
                else
                {
                    await MessageService!.Error(result.Message);
                }
            }
            else
            {
                await MessageService!.Error("校验码错误，请输入最新的校验码重试");
                return;
            }
        }


        private async Task RemoveTOTPAuthenticationSubmit(ApplicationUserMultiFactorAuthenticationDM applicationUserMultiFactorAuthentication)
        {
            await Task.Run(() => {
                ModalService!.Confirm(new ConfirmOptions()
                {
                    Title = "此操作不可撤销!",
                    Content = "确定要清除TOTP令牌吗，清除后需要重新配置",
                    OnOk = async (arg) =>
                    {
                        ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.Remove(applicationUserMultiFactorAuthentication);
                        StateHasChanged();
                        var result = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM, true);
                        if (result.IsSuccess)
                        {
                            await UpdatedApplicationUser.InvokeAsync();
                            await MessageService!.Success(result.Message);
                        }
                        else
                        {
                            await MessageService!.Error(result.Message);
                        }
                    },
                    OkType = "danger",
                });
            }) ;
        }

        private async Task LoadUser()
        {
            await UpdatedApplicationUser.InvokeAsync();
        }

        private async Task SaveUserUpdate()
        {
            var result = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM!, true);
            if (result.IsSuccess)
            {
                await UpdatedApplicationUser.InvokeAsync();
                await MessageService!.Success(result.Message);
            }
            else
            {
                await MessageService!.Error(result.Message);
            }
        }

        private string MFASecretCreate()
        {
            return Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");
        }

        private async Task HandleShortMessageValidationCodeMultiFactorAuthentication()
        {
            DataAccessResult result = new DataAccessResult();
            if(ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.Any(x=>x.ApplicationUserMultiFactorAuthenticationType == ApplicationUserMultiFactorAuthenticationType.短信验证码))
            {
                var applicationUserMultiFactorAuthentication = ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.First(x => x.ApplicationUserMultiFactorAuthenticationType == ApplicationUserMultiFactorAuthenticationType.短信验证码);
                applicationUserMultiFactorAuthentication!.IsEnable = applicationUserMultiFactorAuthentication!.IsEnable;
                applicationUserMultiFactorAuthentication!.Id = 0;
                applicationUserMultiFactorAuthentication!.Secret = MFASecretCreate();
                result = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM, true);
            }
            else
            {
                var newData = new ApplicationUserMultiFactorAuthenticationDM()
                {
                    ApplicationUserId = ApplicationUserDCM!.Id,
                    ApplicationUserMultiFactorAuthenticationType = ApplicationUserMultiFactorAuthenticationType.短信验证码,
                    IsEnable = true,
                    CreateTime = DateTime.Now,
                    Secret = MFASecretCreate()
                };
                ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.Add(newData);
                StateHasChanged();
                result = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM, true);
            }

            if (result.IsSuccess)
            {
                await UpdatedApplicationUser.InvokeAsync();
                await MessageService!.Success(result.Message);
            }
            else
            {
                await MessageService!.Error(result.Message);
            }
        }

        private async Task HandleEmailValidationCodeMultiFactorAuthentication()
        {
            DataAccessResult result = new DataAccessResult();
            if (ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.Any(x => x.ApplicationUserMultiFactorAuthenticationType == ApplicationUserMultiFactorAuthenticationType.邮箱验证码))
            {
                var applicationUserMultiFactorAuthentication = ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.First(x => x.ApplicationUserMultiFactorAuthenticationType == ApplicationUserMultiFactorAuthenticationType.邮箱验证码);
                applicationUserMultiFactorAuthentication!.IsEnable = applicationUserMultiFactorAuthentication!.IsEnable;
                applicationUserMultiFactorAuthentication!.Id = 0;
                applicationUserMultiFactorAuthentication!.Secret = MFASecretCreate();
                result = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM, true);
            }
            else
            {
                var newData = new ApplicationUserMultiFactorAuthenticationDM()
                {
                    ApplicationUserId = ApplicationUserDCM!.Id,
                    ApplicationUserMultiFactorAuthenticationType = ApplicationUserMultiFactorAuthenticationType.邮箱验证码,
                    IsEnable = true,
                    CreateTime = DateTime.Now,
                    Secret = MFASecretCreate()
                };
                ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications!.Add(newData);
                StateHasChanged();
                result = await ApplicationUserService!.SetAndSaveEntityData(ApplicationUserDCM, true);
            }

            if (result.IsSuccess)
            {
                await UpdatedApplicationUser.InvokeAsync();
                await MessageService!.Success(result.Message);
            }
            else
            {
                await MessageService!.Error(result.Message);
            }
        }
    }
}

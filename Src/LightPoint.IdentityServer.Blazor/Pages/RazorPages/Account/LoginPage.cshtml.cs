using AntDesign;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Imps;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Security.MFA.GoogleAuthenticator;
using LightPoint.IdentityServer.ServerInfrastructure.ValidationCode;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.RazorPages.Account
{
    public class LoginPageModel : LightPointPageModel
    {
        private readonly ILogger<LoginPageModel> _logger;
        private readonly IIdentityServerAccountPageService _identityServerAccountPageService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IAntiforgery _antiforgery;
        private readonly IValidationCodeCreater _validationCodeCreater;
        private readonly IConfidentialService _confidentialService;
        private readonly GoogleAuthenticator _googleAuthenticator;

        public LoginPageModel(ILogger<LoginPageModel> logger, IIdentityServerAccountPageService identityServerAccountPageService,
            IApplicationUserService applicationUserService, IAntiforgery antiforgery, IValidationCodeCreater validationCodeCreater,
            IConfidentialService confidentialService) : base(antiforgery)
        {
            _logger = logger;
            _identityServerAccountPageService = identityServerAccountPageService;
            _applicationUserService = applicationUserService;
            _antiforgery = antiforgery;
            _validationCodeCreater = validationCodeCreater;
            _confidentialService = confidentialService;
            _googleAuthenticator = new GoogleAuthenticator(TimeSpan.FromMinutes(1));
        }

        public virtual void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiForgeryToken = tokens.RequestToken;
        }

        public async Task<IActionResult> OnPostAsync(LoginInputModel loginModel)
        {
            if (loginModel.LoginType == LoginType.账号密码)
            {
                // 账号密码登录
                if (string.IsNullOrWhiteSpace(loginModel.Username) || string.IsNullOrWhiteSpace(loginModel.Password))
                {
                    // 这里属于是异常的范畴了
                    return BadRequest();
                }
                else
                {
                    if ((await _applicationUserService!.ValidateCredentials(loginModel.Username!, loginModel.Password!)).IsSuccess)
                    {
                        var user = await _applicationUserService.GetApiBoAsync(x => x.UserName == loginModel.Username, true);

                        if (user!.MutilFactorEnabled && user.MutilFactorEnabled && user.ApplicationUserMultiFactorAuthentications!.Any(x => x.IsEnable))
                        {
                            var applicationUserMultiFactorAuthentications = user.ApplicationUserMultiFactorAuthentications!.Where(x => x.IsEnable).OrderBy(x => x.Id);
                            var secret = "";
                            foreach (var applicationUserMultiFactorAuthentication in applicationUserMultiFactorAuthentications)
                            {
                                var tempData = await _confidentialService.DecryptModelConfidentialPropsAsync(applicationUserMultiFactorAuthentication!);
                                secret += tempData.Secret!;
                            }
                            if (!_googleAuthenticator.ValidateTwoFactorPIN(secret, loginModel.MFAToken!))
                            {
                                return BadRequest();
                            }
                        }
                        else if (user!.TwoFactorEnabled && user.MutilFactorEnabled && user.ApplicationUserMultiFactorAuthentications!.Any(x => x.IsEnable))
                        {
                            var tempData = await _confidentialService.DecryptModelConfidentialPropsAsync(user.ApplicationUserMultiFactorAuthentications!.FirstOrDefault(x => x.ApplicationUserMultiFactorAuthenticationType == loginModel.TowFactorAuthenticationType)!);
                            var secret = tempData!.Secret!;
                            if (!_googleAuthenticator.ValidateTwoFactorPIN(secret, loginModel.MFAToken!))
                            {
                                return BadRequest();
                            }
                        }
                        return await _identityServerAccountPageService.SignIn(this, user!, loginModel);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            else if (loginModel.LoginType == LoginType.短信验证码)
            {
                var validateResult = await _validationCodeCreater!.ValidateValidationCode("PhoneNumberLogin", loginModel.PhoneNumber!, loginModel.ShortMessageCode!);
                if (validateResult)
                {
                    var user = await _applicationUserService.GetApiBoAsync(x => x.PhoneNumber == loginModel.PhoneNumber && x.PhoneNumberConfirmed, true);
                    return await _identityServerAccountPageService.SignIn(this, user!, loginModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (loginModel.LoginType == LoginType.邮箱验证码)
            {
                var validateResult = await _validationCodeCreater!.ValidateValidationCode("EmailLogin", loginModel.Email!, loginModel.EmailCode!);
                if (validateResult)
                {
                    var user = await _applicationUserService.GetApiBoAsync(x => x.Email == loginModel.Email && x.EmailConfirmed);
                    return await _identityServerAccountPageService.SignIn(this, user!, loginModel);
                }
                else
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }
    }
}

using AntDesign;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE03.IdentityServerResources;
using LightPoint.IdentityServer.Shared.Helpers;
using static LightPoint.IdentityServer.Shared.Helpers.OidcConstants;

namespace LightPoint.IdentityServer.Blazor.Pages.ResourcesOperationMaintenanceBusiness.IdentitySeverResourcesBusiness
{
    partial class IdentityServerClientPage : PageBase<Guid, IdentityServerClient, IdentityServerClientDQM, IdentityServerClientDCM, IIdentityServerClientService>
    {
        [Inject]
        public IIdentityServerApiScopeService? ApiScopeService { get; set; }

        public List<IdentityServerApiScopeDQM>? IdentityServerApiScopes { get; set; }

        public override TableParameters<IdentityServerClientDQM> TableParameters { get; set; } = new TableParameters<IdentityServerClientDQM>()
        {
            Length = 15,
            TableColumnInfos = new List<TableColumnInfo>()
            {
                new TableColumnInfo(){ Prop = nameof(IdentityServerClientDQM.ClientId), Header = "客户端Id", Sortable = true, Width = "200px"},
                new TableColumnInfo(){ Prop = nameof(IdentityServerClientDQM.Name), Header = "名称", Sortable = true, Width = "200px"},
                new TableColumnInfo(){ Prop = nameof(IdentityServerClientDQM.TenantIdentifier), Header = "租户标识", Sortable = true},
                new TableColumnInfo(){ Prop = nameof(IdentityServerClientDQM.SortCode), Header = "排序码", Sortable = true},
                new TableColumnInfo(){ Prop = nameof(IdentityServerClientDQM.Description), Header = "简介"},
                new TableColumnInfo(){ Prop = "Operation", Header = "操作", HasSlot = true},
            },
        };

        public override List<FormItemConfig> FormItemConfigs { get; set; } = new List<FormItemConfig>()
        {
            new FormItemConfig(){ FormItemType = FormItemType.文本框, Label = "ClientId", Placeholder = "请输入客户端Id", Prop = nameof(IdentityServerClientDCM.ClientId), FormValidationRules = Rules.Required, Span = 8 },
            new FormItemConfig(){ FormItemType = FormItemType.文本框, Label = "名字", Placeholder = "请输入名字", Prop = "Name", FormValidationRules = Rules.Required, Span = 8},
            new FormItemConfig(){ FormItemType = FormItemType.文本框, Label = "排序码", Placeholder = "请输入排序码", Prop = "SortCode", FormValidationRules = Rules.Required, Span = 8 },
            new FormItemConfig(){ FormItemType = FormItemType.文本框, Label = "Logo地址", Placeholder = "请输入Logo地址", Prop = nameof(IdentityServerClientDCM.LogoUri), Span = 10},
            new FormItemConfig(){ FormItemType = FormItemType.文本框, Label = "登录界面背景图片地址", Placeholder = "请输入登录界面背景图片地址", Prop = nameof(IdentityServerClientDCM.LoginPageBackgroundImgUri), Span = 10},
            new FormItemConfig(){ FormItemType = FormItemType.下拉单选框, Label = "页面模板", Placeholder = "请选择页面模板", Prop = nameof(IdentityServerClientDCM.PageTemplate), Options = new List<LightPointSelectOption>(){
                new LightPointSelectOption(){ Label = "默认", Value = ""},
            }, Span = 4 },
            new FormItemConfig(){ FormItemType = FormItemType.自定义组件, Label = "AllowedCorsOrigins", Placeholder = "请输入AllowedCorsOrigins", Prop = nameof(IdentityServerClientDCM.AllowedCorsOrigins), Span = 24},
            new FormItemConfig(){ FormItemType = FormItemType.自定义组件, Label = "RedirectUris", Placeholder = "请输入RedirectUris", Prop = nameof(IdentityServerClientDCM.RedirectUris), Span = 24},
            new FormItemConfig(){ FormItemType = FormItemType.自定义组件, Label = "PostLogoutRedirectUris", Placeholder = "请输入PostLogoutRedirectUris", Prop = nameof(IdentityServerClientDCM.PostLogoutRedirectUris), Span = 24},
            new FormItemConfig(){ FormItemType = FormItemType.自定义组件, Label = "ClientSecrets", Placeholder = "请输入ClientSecrets", Prop = nameof(IdentityServerClientDCM.ClientSecrets), Span = 24},
            new FormItemConfig(){ FormItemType = FormItemType.文本框, Label = "令牌有效时长（秒）", Placeholder = "请输入令牌有效时长（秒）", Prop = nameof(IdentityServerClientDCM.AccessTokenLifetime), Span = 8},
            new FormItemConfig(){ FormItemType = FormItemType.下拉多选框, Label = "支持的授权类型", Placeholder = "请选择支持的授权类型", Prop = nameof(IdentityServerClientDCM.AllowedGrantTypes), FormValidationRules = Rules.ListRequired, Options = new List<LightPointSelectOption>(){
                new LightPointSelectOption(){ Label = "授权码授权", Value = GrantType.AuthorizationCode},
                new LightPointSelectOption(){ Label = "客户端授权", Value = GrantType.ClientCredentials},
                new LightPointSelectOption(){ Label = "混合流授权", Value = GrantType.Hybrid},
                new LightPointSelectOption(){ Label = "隐式授权", Value =  GrantTypes.Implicit},
                new LightPointSelectOption(){ Label = "资源持有者密码授权", Value = GrantType.ResourceOwnerPassword},
                new LightPointSelectOption(){ Label = "设备授权", Value = GrantType.DeviceFlow },
            }, Span = 8 },
            new FormItemConfig(){ FormItemType = FormItemType.下拉单选框, Label = "颁发的令牌类型", Placeholder = "请选择颁发的令牌类型", Prop = nameof(IdentityServerClientDCM.AccessTokenType), Options = new List<LightPointSelectOption>(){
                new LightPointSelectOption(){ Label = "独立令牌（Jwt）", Value = (int)AccessTokenType.Jwt},
                new LightPointSelectOption(){ Label = "引用令牌（Reference）", Value = (int)AccessTokenType.Reference},
            }, Span = 8 },
            new FormItemConfig(){ FormItemType = FormItemType.多行文本框, Label = "支持的Scope", Placeholder = "请填写支持的Scope，每个scope之间使用英文逗号分隔", Prop = nameof(IdentityServerClientDCM.ScopesFormValueHelper), Span =24},
            new FormItemConfig(){ FormItemType = FormItemType.开关, Label = "允许令牌停留在浏览器", Placeholder = "允许令牌停留在浏览器", Prop = nameof(IdentityServerClientDCM.AllowAccessTokensViaBrowser),Span = 4},
            new FormItemConfig(){ FormItemType = FormItemType.开关, Label = "是否使用密码", Placeholder = "是否使用密码", Prop = nameof(IdentityServerClientDCM.RequireClientSecret), Span = 4},
            new FormItemConfig(){ FormItemType = FormItemType.开关, Label = "是否强制PKCE模式", Placeholder = "是否强制PKCE模式", Prop = nameof(IdentityServerClientDCM.RequirePkce), Span = 4},
            new FormItemConfig(){ FormItemType = FormItemType.开关, Label = "是否允许记录授权", Placeholder = "是否允许记录授权", Prop = nameof(IdentityServerClientDCM.AllowRememberConsent), Span = 4},
            new FormItemConfig(){ FormItemType = FormItemType.开关, Label = "登录是否需要授权", Placeholder = "登录是否需要授权", Prop = nameof(IdentityServerClientDCM.RequireConsent), Span = 4},
            new FormItemConfig(){ FormItemType = FormItemType.开关, Label = "是否启用", Placeholder = "是否启用", Prop = nameof(IdentityServerClientDCM.Enabled), Span = 4, },
            new FormItemConfig(){ FormItemType = FormItemType.多行文本框, Label = "备注", Placeholder = "请输入备注", Prop = nameof(IdentityServerClientDCM.Description) },
        };


        private List<LightPointSelectOption> _SecretTypeOptions = new List<LightPointSelectOption>()
        {
            new LightPointSelectOption() { Label = "SharedSecret", Value = IdentityServerResourcesConstants.SecretTypes.SharedSecret},
            new LightPointSelectOption() { Label = "X509CertificateBase64", Value = IdentityServerResourcesConstants.SecretTypes.X509CertificateBase64},
            new LightPointSelectOption() { Label = "X509CertificateThumbprint", Value = IdentityServerResourcesConstants.SecretTypes.X509CertificateThumbprint},
            new LightPointSelectOption() { Label = "JsonWebKey", Value = IdentityServerResourcesConstants.SecretTypes.JsonWebKey},
        };

		protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await Search();
            }

            base.OnAfterRender(firstRender);
        }


        public override async Task OpenForm(IdentityServerClientDQM? queryDto)
        {
            await Task.Run(() =>
            {
                if (this.LightPointFormRef == null)
                {
                    return;
                }
                if (queryDto != null)
                {
                    var dcm = Mapper<IdentityServerClientDQM, IdentityServerClientDCM>.MapToNewObj(queryDto);
                    if (dcm!.AllowedScopes != null)
                    {
                        dcm!.ScopesFormValueHelper = string.Join(",", dcm.AllowedScopes);
                    }
                    this.LightPointFormRef.Open(dcm);
                }
                else
                {
                    this.LightPointFormRef.Open();
                }
            });
        }

        public override Task Update(IdentityServerClientDCM commandDtoBase)
        {
            if (commandDtoBase.ScopesFormValueHelper != null)
            {
                commandDtoBase.AllowedScopes = commandDtoBase.ScopesFormValueHelper.Split(",").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            }
            return base.Update(commandDtoBase);
        }

        private void _AddAllowedCorsOrigin(IdentityServerClientDCM model)
        {
            if (model.AllowedCorsOrigins == null)
            {
                model.AllowedCorsOrigins = new List<string>() { "" };
            }
            else
            {
                if (model.AllowedCorsOrigins!.Any(x => x == ""))
                {
                    return;
                }
                model.AllowedCorsOrigins.Add("");
            }
        }

        private void _AddRedirectUris(IdentityServerClientDCM model)
        {
            if (model.RedirectUris == null)
            {
                model.RedirectUris = new List<string>() { "" };
            }
            else
            {
                if (model.RedirectUris!.Any(x => x == ""))
                {
                    return;
                }
                model.RedirectUris.Add("");
            }
        }

        private void _AddPostLogoutRedirectUris(IdentityServerClientDCM model)
        {
            if (model.PostLogoutRedirectUris == null)
            {
                model.PostLogoutRedirectUris = new List<string>() { "" };
            }
            else
            {
                if (model.PostLogoutRedirectUris!.Any(x => x == ""))
                {
                    return;
                }
                model.PostLogoutRedirectUris.Add("");
            }
        }

        private void _AddClientSecrets(IdentityServerClientDCM model)
        {
            if (model.ClientSecrets == null)
            {
                model.ClientSecrets = new List<IdentityServerClientSecretDM>() { new IdentityServerClientSecretDM() };
            }
            else
            {
                if (model.ClientSecrets!.Any(x => x.Value == ""))
                {
                    return;
                }
                model.ClientSecrets.Add(new IdentityServerClientSecretDM());
            }
        }

    }
}

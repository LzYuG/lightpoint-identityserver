using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Imps;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.IdentityServer4Imps
{
    public class HostProfileService : IProfileService
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenantInfo _tenantInfo;

        public HostProfileService(IApplicationUserService applicationUserService, ILogger<HostProfileService> logger,
            IHttpContextAccessor httpContextAccessor, TenantInfo tenantInfo)
        {
            _applicationUserService = applicationUserService;
            _httpContextAccessor = httpContextAccessor;
            _tenantInfo = tenantInfo;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            


            string id = context.Subject.GetSubjectId();
            if (Guid.TryParse(id, out Guid userId))
            {
                var user = await _applicationUserService.GetApiBoAsync(x => x.Id == userId);

                if(user != null)
                {
                    // ±ê×¼openid connect×Ö¶Î
                    var standClaims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Name,  user!.Name!),
                    };
                    // roles 
                    var roles = await _applicationUserService.InRoles(userId, _tenantInfo.TenantIdentifier!);
                    standClaims.AddRange(roles.Select(x => new Claim(JwtClaimTypes.Role, x)));

                    if (string.IsNullOrEmpty(user?.PhoneNumber))
                    {
                        standClaims.Add(new Claim(JwtClaimTypes.PhoneNumber, user!.PhoneNumber!));
                        standClaims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString(), ClaimValueTypes.Boolean));
                    }
                    if (string.IsNullOrEmpty(user?.Email))
                    {
                        standClaims.Add(new Claim(JwtClaimTypes.PhoneNumber, user!.Email!));
                        standClaims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, user.EmailConfirmed.ToString(), ClaimValueTypes.Boolean));
                    }

                    if (context.RequestedClaimTypes.Any())
                    {
                        if (user != null)
                        {
                            context.AddRequestedClaims(user.ApplicationUserClaims!.Select(x => new Claim(x.ClaimType!, x.ClaimValue!)));
                        }
                    }

                    context.AddRequestedClaims(standClaims);

                    // custom claims
                    var cliams = await _applicationUserService.GetClaims(userId, _tenantInfo.TenantIdentifier!);
                    context.IssuedClaims.AddRange(cliams.Select(x => new Claim(x.ClaimType!, x.ClaimValue!)));
                }
            }

            var ip = _httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            if (!string.IsNullOrWhiteSpace(ip))
            {
                context.IssuedClaims.Add(new Claim("ip", ip));
            }

            

            var transaction = context.RequestedResources.ParsedScopes.FirstOrDefault(x => x.ParsedName == "transaction");
            if (transaction?.ParsedParameter != null)
            {
                context.IssuedClaims.Add(new Claim("transaction_id", transaction.ParsedParameter));
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string id = context.Subject.GetSubjectId();
            if (Guid.TryParse(id, out Guid userId))
            {
                var user = await _applicationUserService.GetApiBoAsync(x => x.Id == userId);
                if (user != null && user.IsEnable && user.LockoutEnabled)
                {
                    context.IsActive = true;
                    return;
                }
                context.IsActive = false;
            }
            else
            {
                context.IsActive = false;
            }
        }
    }
}
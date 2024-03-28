using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;

namespace LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 用户标识
    /// </summary>
    public class ApplicationUserClaimDM : DtoBase<int>
    {
        //
        // 摘要:
        //     Gets or sets the primary key of the user associated with this claim.
        public virtual Guid UserId { get; set; }

        //
        // 摘要:
        //     Gets or sets the claim type for this claim.
        public virtual string? ClaimType { get; set; }

        //
        // 摘要:
        //     Gets or sets the claim value for this claim.
        public virtual string? ClaimValue { get; set; }
    }
}

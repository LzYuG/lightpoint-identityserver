using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoModels.DM00.Common;

namespace LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources
{
    public class ApplicationRoleDQM : QueryDto<Guid>
    {
        /// <summary>
        /// 是否根角色
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public List<string>? Properties { get; set; }

        //
        // 摘要:
        //     Gets or sets the normalized name for this role.
        public virtual string? NormalizedName { get; set; }

        //
        // 摘要:
        //     A random value that should change whenever a role is persisted to the store
        public virtual string? ConcurrencyStamp { get; set; }
        /// <summary>
        /// 示例，标签
        /// </summary>
        [ExtensionProperty]
        public string? Tag { get; set; }
    }
}

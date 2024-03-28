using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;

namespace LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 用户与角色的多对多关联
    /// </summary>
    public class ApplicationUserWithRoleDM : IDtoBase<Guid>
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public string? SortCode { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否已经删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 租户
        /// </summary>
        public string? TenantIdentifier { get; set; }


        //
        // 摘要:
        //     Gets or sets the primary key of the user that is linked to a role.
        public Guid UserId { get; set; }

        //
        // 摘要:
        //     Gets or sets the primary key of the role that is linked to the user.
        public Guid RoleId { get; set; }

        public async Task<ICommandDtoBase<Guid>?> MapperToCommandDto()
        {
            return await Task.Run(() => { return this; });
        }

        public async Task<IQueryDtoBase<Guid>?> MapperToQueryDto()
        {
            return await Task.Run(() => { return this; });
        }
    }
}

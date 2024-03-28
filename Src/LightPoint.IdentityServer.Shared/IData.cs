namespace LightPoint.IdentityServer.Shared
{
    /// <summary>
    /// 这将是任何数据模型的基架，我们约定每一个业务模型都应该从这个接口开始派生
    /// 可以为int,string,Guid
    /// 目前Id仅支持这些类型
    /// </summary>
    public interface IData<T>
    {
        /// <summary>
        /// Id，可以为int,string,Guid
        /// </summary>
        T Id { get; set; }
        string? SortCode { get; set; }
        DateTime CreateTime { get; set; }
        bool IsDeleted { get; set; }

        string? TenantIdentifier { get; set; }
    }
}

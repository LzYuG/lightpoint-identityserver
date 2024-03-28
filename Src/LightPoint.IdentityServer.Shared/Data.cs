using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Shared
{
    /// <summary>
    /// ShikraFW的基架类，每一个业务模型都将从这里派生
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Data<T> : IData<T>
    {
        [Key]
        //如果是int类型的话将会自增即自生成Id
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; } = default!;
        public DateTime CreateTime { get; set; }
        public string? SortCode { get; set; }

        public bool IsDeleted { get; set; }

        public string? TenantIdentifier { get; set; }
    }
}

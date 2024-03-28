using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM01.SystemResource
{
    public class SystemCommonFileDM : DtoBase<Guid>
    {
        public string? Name { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string? ExpandName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public float Size { get; set; }
        /// <summary>
        /// 文件更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 存储路径
        /// </summary>
        public string? FilePath { get; set; }
        /// <summary>
        /// 存储服务的ip
        /// </summary>
        public string? ServerIP { get; set; }
        /// <summary>
        /// 存储服务名称
        /// </summary>
        public string? ServerName { get; set; }

        public string? UploadPersonName { get; set; }

        public Guid UploadPersonId { get; set; }
        /// <summary>
        /// 临时文件
        /// </summary>
        public bool IsTemp { get; set; }
        /// <summary>
        /// 是外部的文件，则FilePath属性应是url用于获取文件
        /// </summary>
        public bool IsExternalFile { get; set; }
        /// <summary>
        /// 最后一次的访问时间
        /// 文件服务会定时清理长时间未访问的文件
        /// 具体看服务能承载的层次决定时间
        /// </summary>
        public DateTime LastAccessTime { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileExtensionEnum FileExtensionEnum { get; set; }
        public string? FileExtensionEnumName { get; set; }
    }
}

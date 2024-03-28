using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Shared.DataProcessingFacades
{
    public class SearchParams
    {
        /// <summary>
        /// 开始索引
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// 取数据数量
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 是否正序
        /// </summary>
        public bool IsDesc { get; set; }
        /// <summary>
        /// 排序属性
        /// </summary>
        public string OrderProp { get; set; } = "SortCode";
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string? SearchTerm { get; set; }
        /// <summary>
        /// 扩展参数1
        /// </summary>
        public string? ExtensionValue1 { get; set; }
        /// <summary>
        /// 扩展参数2
        /// </summary>
        public string? ExtensionValue2 { get; set; }
        /// <summary>
        /// 扩展参数3
        /// </summary>
        public string? ExtensionValue3 { get; set; }
        /// <summary>
        /// 扩展参数4
        /// </summary>
        public string? ExtensionValue4 { get; set; }
        /// <summary>
        /// 扩展参数5
        /// </summary>
        public string? ExtensionValue5 { get; set; }
        /// <summary>
        /// 是否需要层级关系映射
        /// </summary>
        public bool IsIndent { get; set; }
    }
}

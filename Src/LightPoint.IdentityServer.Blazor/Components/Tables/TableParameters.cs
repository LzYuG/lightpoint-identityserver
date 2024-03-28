using AntDesign;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;

namespace LightPoint.IdentityServer.Blazor.Components.Tables
{
    public class TableParameters<TModel> where TModel : class, new()
    {
        public TableParameters()
        {
        }

        public TableParameters(int pageIndex, int length, List<TableColumnInfo> tableColumnInfos)
        {
            PageIndex = pageIndex;
            Start = (pageIndex - 1) * length;
            Length = length;
            TableColumnInfos = tableColumnInfos;
        }

        /// <summary>
        /// 数据开始索引
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// 页面数据长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string? SearchTerm { get; set; }
        /// <summary>
        /// 排序属性
        /// </summary>
        public string OrderProp { get; set; } = "SortCode";
        /// <summary>
        /// 是否加载中
        /// </summary>
        public bool Loading { get; set; }
        /// <summary>
        /// 是否倒序
        /// </summary>
        public bool IsDesc { get; set; }
        /// <summary>
        /// 一些预留的可以跟表格参数统一提交的页面筛选参数
        /// </summary>
        public object? ExtensionValue1 { get; set; }
        public object? ExtensionValue2 { get; set; }
        public object? ExtensionValue3 { get; set; }
        public object? ExtensionValue4 { get; set; }
        public object? ExtensionValue5 { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<TModel>? Datas { get; set; } = new List<TModel>();
        public int Total { get; set; } = 151;
        public List<TModel>? SelectedDatas { get; set; } = new List<TModel>();

        /// <summary>
        /// 用于筛选的方法
        /// params1: ref model
        /// params2: keyWord
        /// </summary>
        public Func<TModel, bool>? FilterFunc { get; set; } = (model) => true;
        /// <summary>
        /// Size
        /// </summary>
        public TableSize Size { get; set; } = TableSize.Small;
        /// <summary>
        /// 边框
        /// </summary>
        public bool Bordered { get; set; } = true;
        /// <summary>
        /// 开启多选
        /// </summary>
        public bool MultiSelection { get; set; } = true;
        /// <summary>
        /// 横向固定时，配置的最大值
        /// </summary>
        public string? ScrollX { get; set; }
        /// <summary>
        /// 是否需要分页
        /// </summary>
        public bool NeedPager { get; set; } = true;

        public List<TableColumnInfo>? TableColumnInfos { get; set; }

        #region 分页参数
        public int PageIndex { get; set; } = 1;

        public List<int> PageSizeOptions { get; set; } = new List<int>() { 15, 25, 30 };
        #endregion



        public SearchParams ToRequestParams()
        {
            return new SearchParams()
            {
                Start = Start,
                Length = Length,
                IsDesc = IsDesc,
                OrderProp = OrderProp,
                SearchTerm = SearchTerm,
                ExtensionValue1 = ExtensionValue1 as string,
                ExtensionValue2 = ExtensionValue2 as string,
                ExtensionValue3 = ExtensionValue3 as string,
                ExtensionValue4 = ExtensionValue4 as string,
                ExtensionValue5 = ExtensionValue5 as string,
            };
        }
    }
}

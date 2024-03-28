namespace LightPoint.IdentityServer.Shared.DataProcessingFacades
{
    public class TableDataHelper<T>
    {
        /// <summary>
        /// 经过筛选后得到的数据总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T>? Datas { get; set; }
    }
}

namespace LightPoint.IdentityServer.Shared.DataProcessingFacades
{
    public class DataAccessResult
    {
        /// <summary> 
        public string? Message { get; set; }
        /// <summary>
        /// 操作返回类型
        /// </summary>
        public DataAccessResultEnum DataStatusEnum { get; set; }

        public string? OtherParameter { get; set; }

        public bool IsSuccess
        {
            get => DataStatusEnum == DataAccessResultEnum.操作成功;
        }

        public static DataAccessResult Error(string message = "操作失败")
        {
            return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = message };
        }

        public static DataAccessResult Success(string message = "操作成功")
        {
            return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作成功, Message = message };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LightPoint.IdentityServer.Shared.Extensions
{
    public class SortCodeCreater
    {
        public static string CreateSortCode(string className)
        {
            //根据当前时间生成字符串用以排序
            return className + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff").Replace("-", "").Replace(" ", "").Replace("T", "").Replace(":", "");
        }
    }
}

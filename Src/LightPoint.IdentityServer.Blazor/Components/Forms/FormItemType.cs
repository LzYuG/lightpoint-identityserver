using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms
{
    public enum FormItemType
    {
        文本框,
        密码输入框,
        整数文本框,
        多行文本框,
        下拉单选框,
        /// <summary>
        /// 下拉多选框如果绑定的是string，则需要配置Transform = (obj) => string.Join(",", (List<string>)obj)
        /// </summary>
        下拉多选框,
        日期选择器,
        日期区间选择器,
        周选择器,
        月份选择器,
        年份选择器,
        时间选择器,
        日期时间选择器,
        日期时间区间选择器,
        复选框,
        单选框,
        开关,
        填充块,
        自定义组件
    }


}

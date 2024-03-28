using LightPoint.IdentityServer.Blazor.Layout.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Layout.Helpers
{
    public interface ILayoutInitor
    {
        Task<List<ApplicationMenu>> GetMenus();

        /// <summary>
        /// 获取用户头像所需数据
        /// </summary>
        /// <returns></returns>
        Task<AvatarModel> GetUserInfo();
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        Task Logout();
        /// <summary>
        /// 导航至资料页面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task GoProfile(AvatarModel model);
    }
}

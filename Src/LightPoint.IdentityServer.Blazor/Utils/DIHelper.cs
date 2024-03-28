using Microsoft.Extensions.DependencyInjection;
using LightPoint.IdentityServer.Blazor.Layout.Helpers;
using LightPoint.IdentityServer.Blazor.Utils.Modules.Tools;
using Blazored.LocalStorage;

namespace LightPoint.IdentityServer.Blazor.Utils
{
    public static class DIHelper
    {
        public static void FrameworkDependentInject<TLayoutInitor>(this IServiceCollection services)
            where TLayoutInitor : class, ILayoutInitor
        {
            services.AddScoped<ILayoutInitor, TLayoutInitor>();
            services.AddScoped<ToolsJsInterop>();
            services.AddScoped<Modals>();
            services.AddScoped<Globals>();
            services.AddAntDesign();
			services.AddBlazoredLocalStorage();
		}
    }
}

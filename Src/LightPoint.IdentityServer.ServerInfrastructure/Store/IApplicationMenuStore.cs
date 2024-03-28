using LightPoint.IdentityServer.ServerInfrastructure.Store.Models;

namespace LightPoint.IdentityServer.ServerInfrastructure.Store
{
    public interface IApplicationMenuStore
    {
        Task<List<ApplicationMenu>> GetMenus();
    }
}

using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.SMS
{
    public interface IShortMessageServerService
    {
        Task SendValidationCodeAsync(TenantInfo tenantInfo, ValidationCodeShortMessageModel model, CancellationToken cancellationToken);
    }
}

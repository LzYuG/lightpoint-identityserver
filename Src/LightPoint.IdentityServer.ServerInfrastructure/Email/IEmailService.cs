using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Email
{
    public interface IEmailService
    {
        Task SendValidationCodeAsync(TenantInfo tenantInfo, ValidationCodeEmailModel mailModel, CancellationToken cancellationToken);
    }
}

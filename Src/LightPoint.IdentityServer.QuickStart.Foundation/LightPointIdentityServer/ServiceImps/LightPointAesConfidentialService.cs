using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.QuickStart.Foundation.LightPointIdentityServer.ServiceImps
{
    public class LightPointAesConfidentialService : ConfidentialServiceBase, IConfidentialService
    {
        public override string AesPassword { get; } = "S0JHr2dZL4ahfYdLq5Xo9KeGaBNW2rC1D5p4dWj5I5A=";
    }
}

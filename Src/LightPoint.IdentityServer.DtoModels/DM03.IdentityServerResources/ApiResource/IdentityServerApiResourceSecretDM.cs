using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource
{
	public class IdentityServerApiResourceSecretDM : IdentityServerSecretDM
	{
		public Guid ApiResourceId { get; set; }
	}
}

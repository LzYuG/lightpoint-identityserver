using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Layout.Header
{
    public class AvatarModel
    {
        public string? UserName { get; set; }

        public string? AvatarUri { get; set; }

        public string? UserId { get; set; }
    }
}

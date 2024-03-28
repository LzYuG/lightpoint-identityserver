using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator.Models
{
    public class VerifyImage
    {
        public string? GapImage { get; set; }
        public string? SliderImage { get; set; }
        public int GapX { get; set; }
        public int GapY { get; set; }

        public int GapWidth { get; set; }
        public int GapHeight { get; set; }
    }
}

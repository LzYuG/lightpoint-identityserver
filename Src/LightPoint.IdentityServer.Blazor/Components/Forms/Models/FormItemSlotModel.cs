using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms.Models
{
    public class FormItemSlotModel<TModel> where TModel : class, new()
    {
        public FormItemConfig? FormItemConfig { get; set; }

        public TModel? Model { get; set; }
    }
}

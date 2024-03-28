using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Tables
{
    public class SlotModel<TModel> where TModel : class, new()
    {
        public TableColumnInfo? ColumnInfo { get; set; }

        public TModel? Model { get; set; }
    }
}

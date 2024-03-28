using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Tables
{
    public class TableResponse<T> where T : class
    {
        public int Total { get; set; }

        public List<T>? Datas { get; set; } = new List<T>();
    }
}

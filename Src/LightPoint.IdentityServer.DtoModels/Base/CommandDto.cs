using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.Base
{
    public class CommandDto<T> : CommandDtoBase<T>, ICommandDto<T>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}

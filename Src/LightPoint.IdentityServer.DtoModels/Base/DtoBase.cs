using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.Base
{
    public class DtoBase<T> : Data<T>, IQueryDtoBase<T>, ICommandDtoBase<T>
    {
    }
}

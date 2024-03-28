using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.ValidationCode
{
    public interface IValidationCodeCreater
    {
        string CreateValidationCodeCacheKey(string type, string operationId);
        Task<ValidationCodeModel> CreateValidationCode(string type, string operationId, int n);
        Task<bool> ValidateValidationCode(string type, string operationId, string code, bool clearWhenValidateSuccessed = true);
    }
}

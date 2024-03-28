using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM00.Common
{
    public class DomainModelValidationResult
    {
        public string? Message { get; set; }

        public bool Successed { get; set; }

        public static DomainModelValidationResult Success(string message = "")
        {
            return new DomainModelValidationResult()
            {
                Message = message,
                Successed = true
            };
        }

        public static DomainModelValidationResult Fail(string message = "校验失败")
        {
            return new DomainModelValidationResult()
            {
                Message = message,
                Successed = false
            };
        }
    }
}

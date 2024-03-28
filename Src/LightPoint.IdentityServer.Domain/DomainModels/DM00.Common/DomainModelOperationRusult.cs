using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM00.Common
{
    public class DomainModelOperationRusult
    {
        public DomainModelOperationRusult()
        {
        }

        public DomainModelOperationRusult(string? message, bool successed)
        {
            Message = message;
            Successed = successed;
        }

        public DomainModelOperationRusult(DataAccessResult dataAccessResult)
        {
            Message = dataAccessResult.Message;
            Successed = dataAccessResult.IsSuccess;
        }

        public string? Message { get; set; }

        public bool Successed { get; set; }

        public static DomainModelOperationRusult Success(string message = "")
        {
            return new DomainModelOperationRusult()
            {
                Message = message,
                Successed = true
            };
        }

        public static DomainModelOperationRusult Fail(string message = "校验失败")
        {
            return new DomainModelOperationRusult()
            {
                Message = message,
                Successed = false
            };
        }
    }
}

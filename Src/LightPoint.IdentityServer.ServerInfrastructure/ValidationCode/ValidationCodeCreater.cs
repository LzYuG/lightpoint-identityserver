using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.ValidationCode
{
    public class ValidationCodeCreater : IValidationCodeCreater
    {
        private readonly ILightPointCache _lightPointCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidationCodeCreater(ILightPointCache lightPointCache, IHttpContextAccessor httpContextAccessor)
        {
            _lightPointCache = lightPointCache;
            _httpContextAccessor = httpContextAccessor;
        }


        public string CreateValidationCodeCacheKey(string type, string operationId)
        {
            var ip = _httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            return nameof(ValidationCodeCreater) + "_" + type + "_" + operationId + "_" + ip;
        }

        public async Task<ValidationCodeModel> CreateValidationCode(string type, string operationId, int n)
        {
            var ip = _httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            var key = CreateValidationCodeCacheKey(type, operationId);
            var code = CreateCharCode(n);
            ValidationCodeModel validationCodeModel = new ValidationCodeModel()
            {
                Code = code,
                ExpireTime = DateTime.Now.AddMinutes(5),
                OperationId = operationId,
                CreateTime = DateTime.Now,
                Ip = ip
            };

            await _lightPointCache.SetItemAsymc(key, validationCodeModel, TimeSpan.FromMinutes(5));
            return validationCodeModel;
        }

        public async Task<bool> ValidateValidationCode(string type, string operationId, string code, bool clearWhenValidateSuccessed = true)
        {
            var ip = _httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            var key = CreateValidationCodeCacheKey(type, operationId);
            var model = await _lightPointCache.GetItemAsymc<ValidationCodeModel>(key);

            if(model != null && model.Code == code && model.Ip == ip)
            {
                if (clearWhenValidateSuccessed)
                {
                    await _lightPointCache.RemoveItemAsymc(key);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="n">验证码数</param>
        /// <returns></returns>
        private static string CreateCharCode(int n)
        {
            char[] strChar = { '0','1','2','3', '4','5','6','7','8','9' };

            string charCode = string.Empty;

            Random random = new Random();

            for (int i = 0; i < n; i++)
            {
                charCode += strChar[random.Next(strChar.Length)];
            }
            return charCode;
        }
    }
}

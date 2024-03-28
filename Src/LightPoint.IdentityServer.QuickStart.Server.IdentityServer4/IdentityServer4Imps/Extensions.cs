using System;
using IdentityModel;
using IdentityServer4;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.IdentityServer4Imps
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if the redirect URI is for a native client.
        /// </summary>
        /// <returns></returns>
        public static bool IsNativeClient(this AuthorizationRequest context)
        {
            return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
        }


        public static IIdentityServerBuilder AddSigningCredential(this IIdentityServerBuilder builder)
        {
            // create random RS256 key
            return builder.AddDeveloperSigningCredential();

            //// use an RSA-based certificate with RS256
            //var rsaCert = new X509Certificate2("./keys/identityserver.test.rsa.p12", "changeit");

            //// ...and PS256 默认使用PS256算法
            //builder.AddSigningCredential(rsaCert, "PS256");

            //// 次选RS256
            //builder.AddSigningCredential(rsaCert, "RS256");



            // or manually extract ECDSA key from certificate (directly using the certificate is not support by Microsoft right now)
            //var ecCert = new X509Certificate2("./keys/identityserver.test.ecdsa.p12", "changeit");
            //var key = new ECDsaSecurityKey(ecCert.GetECDsaPrivateKey())
            //{
            //    KeyId = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)
            //};

            //return builder.AddSigningCredential(
            //    key,
            //    IdentityServerConstants.ECDsaSigningAlgorithm.ES256);
        }
    }
}

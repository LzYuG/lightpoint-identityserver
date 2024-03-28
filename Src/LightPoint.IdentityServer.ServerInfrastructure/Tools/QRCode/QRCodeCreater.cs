using LightPoint.IdentityServer.ServerInfrastructure.Security.MFA.GoogleAuthenticator;
using SkiaSharp.QrCode.Image;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Tools.QRCode
{
    public static class QRCodeCreater
    {
        /// <summary>
        /// 创建二维码，返回图片的base64格式
        /// </summary>
        /// <returns></returns>
        public static string? CreateQRCodeImage(string url, int width)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // generate QRCode
                var qrCode = new QrCode(url, new Vector2Slim(width, width), SKEncodedImageFormat.Png);

                // output to MemoryStream
                qrCode.GenerateImage(ms);

                return String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray()));
            }
        }
    }
}

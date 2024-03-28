using LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator.Models;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Security.Cryptography;

namespace LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator
{
    public class VerifyCodeHelper
    {
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="n">验证码数</param>
        /// <param name="type">类型 0：数字 1：字符</param>
        /// <returns></returns>
        public static VerifyCode CreateVerifyCode(IWebHostEnvironment webHostEnvironment, HumanMachineVerificationType type)
        {
            var rootPath = webHostEnvironment.ContentRootPath.Replace("\\", "/");

            if (webHostEnvironment.IsDevelopment())
            {
                // 本地调试，需要指定文件目录到类库
                var assembly = Assembly.GetExecutingAssembly();
                var lastPair = rootPath.Split("/")[^1];
                var serverAssamblyName = assembly.GetName().Name!;
                rootPath = rootPath.Replace(lastPair, serverAssamblyName);
            }
            rootPath += "/Assets";
            int n = 0;
            //初始化验证码
            string charCode = "";
            var result = "";
            if(type == HumanMachineVerificationType.随机字符校验码)
            {
                n = 4;
                charCode = CreateCharCode(n);
                result = charCode;
            }
            else if(type == HumanMachineVerificationType.数学计算校验码)
            {
                var res = CreateNumCode();
                charCode = res.Item1;
                n = charCode.Length;
                result = res.Item2;
            }

            //宽、高，字体大小
            int codeW = 24 * charCode.Length;
            int codeH = 36;
            int fontSize = 24;


            //颜色列表
            SKColor[] colors = { SKColors.Black, SKColors.Red, SKColors.Blue, SKColors.Green, SKColors.Orange, SKColors.Brown, SKColors.DarkBlue };

            //字体列表
            string[] fonts = { "/Fonts/COMIC.TTF", "/Fonts/GABRIOLA.TTF", "/Fonts/TIMES.TTF" };

            //创建画布
            SKBitmap bitmap = new SKBitmap(codeW, codeH);
            SKCanvas canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            Random random = new Random();

            //画躁线
            for (int i = 0; i < n; i++)
            {
                int x1 = random.Next(codeW);
                int y1 = random.Next(codeH);
                int x2 = random.Next(codeW);
                int y2 = random.Next(codeH);
                SKColor color = colors[random.Next(colors.Length)];
                SKPaint paint = new SKPaint
                {
                    Color = color,
                    StrokeWidth = 1
                };
                canvas.DrawLine(x1, y1, x2, y2, paint);
            }

            //画噪点
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(codeW);
                int y = random.Next(codeH);
                SKColor color = colors[random.Next(colors.Length)];
                bitmap.SetPixel(x, y, color);
            }

            //画验证码
            for (int i = 0; i < n; i++)
            {
                string fontStr = fonts[random.Next(fonts.Length)];
                SKTypeface typeface = SKTypeface.FromFile((rootPath + fontStr).Replace("\\", "/"));
                SKPaint paint = new SKPaint
                {
                    Typeface = typeface,
                    TextSize = fontSize,
                    Color = colors[random.Next(colors.Length)]
                };
                canvas.DrawText(charCode[i].ToString(), i * 15 + 2, fontSize, paint);
            }

            //写入内存流
            try
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Encode(stream, SKEncodedImageFormat.Jpeg, 100);

                VerifyCode verifyCode = new VerifyCode()
                {
                    Result = result,
                    Image = stream.ToArray()
                };
                return verifyCode;
            }

            //释放资源
            finally
            {
                canvas.Dispose();
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// 获取随机数字计算字符串
        /// </summary>
        /// <param name="n">验证码数</param>
        /// <returns>string, result</returns>
        private static Tuple<string, string> CreateNumCode()
        {
            Random random = new Random();

            char[] calculateTypes = { '+', '-', '*' };
            var calculateType = calculateTypes[random.Next(0, 3)];

            var firstNumber = random.Next(1, 99);
            var secendNumber = random.Next(1, 9);

            while(_GetCalculateResult(calculateType, firstNumber, secendNumber) < 0)
            {
                secendNumber = random.Next(1, 9);
            }

            var result = _GetCalculateResult(calculateType, firstNumber, secendNumber);

            return new Tuple<string, string>($"{firstNumber}{calculateType}{secendNumber}=", result.ToString());
        }

        private static int _GetCalculateResult(char calculateType, int firstNumber, int secendNumber)
        {
            if (calculateType == '+')
            {
                return firstNumber + secendNumber;
            }
            else if (calculateType == '-')
            {
                return firstNumber - secendNumber;
            }
            else
            {
                return firstNumber * secendNumber;
            }
        }

        /// <summary>
        /// 获取字符验证码
        /// </summary>
        /// <param name="n">验证码数</param>
        /// <returns></returns>
        private static string CreateCharCode(int n)
        {
            char[] strChar = { 'a', 'b','c','d','e','f','g','h','i','j','k','m',
                'n','p','q','r','s','t','u','v','w','x','y','z','1','2','3',
                '4','5','6','7','8','9','A','B','C','D','E','F','G','H','J','K',
                'L','M','N','P','Q','R','S','T','U','V','W','X','Y','Z'};

            // 去掉一些容易混淆的字符
            // 'l','I','o','O','0',

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

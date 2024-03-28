using LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator
{
    public static class VerifyImageHelper
    {
        public static VerifyImage CreateVerifyImage(IWebHostEnvironment webHostEnvironment, int expectedWidth, int expectedHeigh)
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
            List<string> images = new List<string>() {
                "/VerifyImages/01.jpg",
                "/VerifyImages/02.jpg",
                "/VerifyImages/03.jpg",
                "/VerifyImages/04.jpg",
                "/VerifyImages/05.jpg",
            };
            Random random = new Random();
            var image = images[random.Next(0, images.Count)];

            return _CreateVerifyImage((rootPath + image), expectedWidth, expectedHeigh);
        }

        public static VerifyImage _CreateVerifyImage(string imagePath, int expectedWidth, int expectedHeight)
        {
            // 缺口的宽度和高度
            int gapWidth = 50;
            int gapHeight = 50;
            Console.WriteLine(imagePath);

            // 从文件加载图片
            SKBitmap originalBitmap = SKBitmap.Decode(imagePath);

            // 如果图片的大小不满足期望的宽度和高度，则进行剪切或缩放
            if (originalBitmap.Width != expectedWidth || originalBitmap.Height != expectedHeight)
            {
                originalBitmap = originalBitmap.Resize(new SKImageInfo(expectedWidth, expectedHeight), SKFilterQuality.Medium);
            }

            // 创建带有缺口的图
            SKBitmap gapBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
            SKCanvas gapCanvas = new SKCanvas(gapBitmap);
            gapCanvas.Clear(SKColors.White);

            // 从宽度和高度的最大值中随机选择缺口的位置
            Random random = new Random();
            int gapX = random.Next(gapWidth, expectedWidth - gapWidth);
            int gapY = random.Next(gapHeight, expectedHeight - gapHeight);

            // 定义缺口的形状
            SKRect rect = new SKRect(gapX, gapY, gapX + gapWidth, gapY + gapHeight);

            // 在带有缺口的图上画出原图，并在原图上剪切出缺口
            gapCanvas.DrawBitmap(originalBitmap, 0, 0);
            gapCanvas.DrawRect(rect, new SKPaint { Color = SKColors.White, BlendMode = SKBlendMode.Src });

            // 创建滑块
            SKBitmap sliderBitmap = new SKBitmap(gapWidth, gapHeight);
            SKCanvas sliderCanvas = new SKCanvas(sliderBitmap);
            sliderCanvas.DrawBitmap(originalBitmap, new SKRectI(gapX, gapY, gapX + gapWidth, gapY + gapHeight), new SKRect(0, 0, gapWidth, gapHeight));

            // 将图片保存到内存流中
            MemoryStream gapStream = new MemoryStream();
            gapBitmap.Encode(gapStream, SKEncodedImageFormat.Png, 100);

            MemoryStream sliderStream = new MemoryStream();
            sliderBitmap.Encode(sliderStream, SKEncodedImageFormat.Png, 100);

            // 将内存流转换为Base64字符串
            string gapBase64 = Convert.ToBase64String(gapStream.ToArray());
            string sliderBase64 = Convert.ToBase64String(sliderStream.ToArray());

            // 创建数据对象
            VerifyImage verifyImage = new VerifyImage()
            {
                GapImage = "data:image/jpeg;base64," + gapBase64,
                SliderImage = "data:image/jpeg;base64," + sliderBase64,
                GapX = gapX,
                GapY = gapY,
                GapHeight = gapHeight,
                GapWidth = gapWidth
            };

            return verifyImage;
        }
    }
}

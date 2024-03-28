using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace LightPoint.IdentityServer.ServerInfrastructure.FileStorage
{

    public class FileRepositoryImpByLocalStorage : IFileRepository
    {
        private readonly ILightPointCache _cddCache;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileRepositoryImpByLocalStorage(ILightPointCache cddCache,
            IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _cddCache = cddCache;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public int ExpireSecends { get; set; } = 60 * 60 * 2;

        public string BaseAddress { get; set; } = "https://localhost:6666/";

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    File.Delete(_environment.WebRootPath + filePath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }


        // 在本地的情况下，这个其实应该由服务自行实现，以下只提供参考逻辑
        // 并且服务自身，应提供方法校验发出的令牌
        // 如果要使用本地文件存储，请重写该方法
        public virtual async Task<string> GetFilePathAsync(string filePath)
        {
            var filePathParam = HttpUtility.UrlEncode(filePath);
            // 需要缓存服务支持
            // 简单创建一个id
            var key = Guid.NewGuid();

            // 将key缓存起来
            var cacheKey = filePathParam + key;
            await _cddCache.SetItemAsymc(cacheKey, "1", TimeSpan.FromSeconds(ExpireSecends));

            return _httpContextAccessor.HttpContext!.Request.Scheme + "://" + _httpContextAccessor.HttpContext!.Request.Host + _httpContextAccessor.HttpContext!.Request.Path + "/SharedFile?filePath=" + filePathParam + "&key=" + key;
        }

        public virtual async Task<Stream?> ReadFileAsync(string fileName)
        {
            var imgPath = _environment.WebRootPath + fileName;
            try
            {
                var imgStream = new MemoryStream(await File.ReadAllBytesAsync(imgPath));
                return imgStream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual async Task<bool> SaveFileAsync(Stream file, string filePath)
        {
            // 统一转换成斜杠
            filePath = filePath.Replace("\\", "/");

            var filePatterns = filePath.Split("/").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var fileDir = _environment.WebRootPath.Replace("\\", "/") + "/" + string.Join("/", filePatterns.Take(filePatterns.Count - 1).Select(x => x));

            //创建存储文件夹
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            try
            {
                //文件保存
                using (var fs = File.Create(_environment.WebRootPath.Replace("\\", "/") + filePath))
                {
                    await file.CopyToAsync(fs);
                    await fs.FlushAsync();
                    file.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

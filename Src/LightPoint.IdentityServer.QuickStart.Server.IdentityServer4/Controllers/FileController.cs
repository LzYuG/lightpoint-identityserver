using IdentityServer4.Extensions;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using LightPoint.IdentityServer.ServerInfrastructure.FileStorage;
using LightPoint.IdentityServer.ServerInfrastructure.FileStorage.Models;
using LightPoint.IdentityServer.ServerInfrastructure.FileStorage.Tools;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace YLL.InfomationRelease.MainServer.Controllers
{
    [ApiController]
    [Route("OpenAPI/[controller]/[action]")]
    public class FileController : Controller
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly IAppService<Guid, SystemCommonFile, SystemCommonFileDM, SystemCommonFileDM> _fileService;
        private readonly ILightPointCache _lightPointCache;
        private readonly IFileRepository _fileRepository;
        private readonly TenantInfoAccessor _tenantInfoAccessor;

        public FileController(IApplicationUserService applicationUserService,
            IAppService<Guid, SystemCommonFile, SystemCommonFileDM, SystemCommonFileDM> fileService,
            ILightPointCache lightPointCache, IFileRepository fileRepository, TenantInfoAccessor tenantInfoAccessor)
        {
            _applicationUserService = applicationUserService;
            _fileService = fileService;
            _lightPointCache = lightPointCache;
            _fileRepository = fileRepository;
            _tenantInfoAccessor = tenantInfoAccessor;
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Stream?>> BusinessFile(Guid id)
        {
            var file = await _fileService.GetApiBoAsync(x => x.Id == id && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
            if (file == null) return NotFound();
            await _UpdateLastAccessTime(id);

            if (file.FileExtensionEnum == FileExtensionEnum.PNG || file.FileExtensionEnum == FileExtensionEnum.JPG)
            {
                if (file.IsExternalFile)
                {
                    HttpClient client = new HttpClient();
                    var stream = await client.GetStreamAsync(file.FilePath);
                    return stream;
                }
                try
                {
                    var imgStream = await _fileRepository.ReadFileAsync(file.FilePath!);
                    // 图片直接返回就行
                    return imgStream;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return StatusCode(500);
                }
            }
            else if (file.FileExtensionEnum == FileExtensionEnum.MP4 || file.FileExtensionEnum == FileExtensionEnum.AVI || file.FileExtensionEnum == FileExtensionEnum.FLV)
            {
                if (file.IsExternalFile)
                {
                    HttpClient client = new HttpClient();
                    var stream = await client.GetStreamAsync(file.FilePath);
                    return stream;
                }
                try
                {
                    var imgStream = await _fileRepository.ReadFileAsync(file.FilePath!);
                    string mimeType;
                    switch (file.FileExtensionEnum)
                    {
                        case FileExtensionEnum.MP4:
                            mimeType = "video/mp4";
                            break;
                        case FileExtensionEnum.AVI:
                            mimeType = "video/avi";
                            break;
                        case FileExtensionEnum.FLV:
                            mimeType = "video/x-flv";
                            break;
                        default:
                            mimeType = "application/octet-stream";
                            break;
                    }
                    return File(imgStream!, mimeType, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return StatusCode(500);
                }
            }
            else
            {
                if (file.IsExternalFile)
                {
                    HttpClient client = new HttpClient();
                    var stream = await client.GetStreamAsync(file.FilePath);
                    return stream;
                }
                try
                {
                    // 直接返回文件流，不做解释
                    var imgStream = await _fileRepository.ReadFileAsync(file.FilePath!);
                    return imgStream;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return StatusCode(500);
                }
            }
        }

        // 获取临时文件地址
        [HttpGet]
        [Authorize]
        public async Task<DataAccessResult> FileSharedUrl(Guid id)
        {
            await _GetApplicationUser();
            var file = await _fileService.GetApiBoAsync(x => x.Id == id && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
            if (file == null) return DataAccessResult.Error("无法找到文件");
            await _UpdateLastAccessTime(id);

            var res = await _fileRepository.GetFilePathAsync(file.FilePath!);

            return new DataAccessResult()
            {
                DataStatusEnum = string.IsNullOrEmpty(res) ? DataAccessResultEnum.操作失败 : DataAccessResultEnum.操作成功,
                OtherParameter = res
            };
        }

        // 如果不采用MinIO的情况下，一个临时的图片获取地址
        [HttpGet]
        public async Task<ActionResult<Stream?>> SharedFile(string filePath, string key)
        {
            await _GetApplicationUser();
            var cacheKey = HttpUtility.UrlEncode(filePath) + key;
            if (await _lightPointCache.GetItemAsymc(cacheKey) != null)
            {
                return await _fileRepository.ReadFileAsync(HttpUtility.UrlDecode(filePath));
            }
            return Forbid();
        }


        [HttpPost]
        [Authorize]
        public async Task<FileUploadResponse> UploadFile([FromForm] IFormFile file)
        {
            var user = await _GetApplicationUser();
            var fileType = FileHandle.CheckFileExtensionNames(new List<FileExtensionEnum>()
            {
                FileExtensionEnum.PNG, FileExtensionEnum.JPG, FileExtensionEnum.MP4,
                FileExtensionEnum.AVI, FileExtensionEnum.FLV
            }, file.OpenReadStream());
            if (fileType == null)
            {
                return new FileUploadResponse() { IsSuccess = false, ResultMsg = "上传失败，文件类型校验不通过" };
            }

            var currentDate = DateTime.Now;
            try
            {
                var tenantIdentifier = _tenantInfoAccessor.TenantIdentifier;
                var filePath = $"/UploadFile/{tenantIdentifier}/{currentDate:yyyyMMdd}/";

                if (file != null)
                {
                    //判断文件大小
                    var fileSize = file.Length;

                    if ((fileType == FileExtensionEnum.PNG || fileType != FileExtensionEnum.JPG) &&
                            fileSize > 1024 * 1024 * 10) //10M TODO:(1mb=1024X1024b)
                    {
                        return new FileUploadResponse() { IsSuccess = false, ResultMsg = "图片文件不能大于10M" };
                    }
                    if ((fileType == FileExtensionEnum.MP4) &&
                            fileSize > 1024 * 1024 * 200) //10M TODO:(1mb=1024X1024b)
                    {
                        return new FileUploadResponse() { IsSuccess = false, ResultMsg = "视频文件不能大于500M" };
                    }
                    var fileExtension = System.IO.Path.GetExtension(file.FileName);//获取文件格式，拓展名
                    //保存的文件名称(以名称和保存时间命名)
                    var saveName = Guid.NewGuid().ToString() + "_" + currentDate.ToString("HHmmss") + fileExtension;

                    //完整的文件路径
                    var completeFilePath = System.IO.Path.Combine(filePath, saveName);

                    if (!await _SaveFileAsync(file, completeFilePath))
                    {
                        return new FileUploadResponse() { IsSuccess = false, ResultMsg = "保存图片失败" };
                    }

                    SystemCommonFileDM commonFile = new SystemCommonFileDM()
                    {
                        IsTemp = true,
                        Name = saveName,
                        ExpandName = fileExtension,
                        FilePath = completeFilePath,
                        Id = Guid.NewGuid(),
                        TenantIdentifier = tenantIdentifier,
                        UploadPersonName = user!.Name,
                        UploadPersonId = user.Id,
                        Size = fileSize,
                        UpdateTime = DateTime.Now,
                        FileExtensionEnum = fileType.Value,
                        FileExtensionEnumName = fileType.Value.ToString(),
                        LastAccessTime = DateTime.Now,
                    };
                    var result = await _fileService.SetAndSaveEntityData(commonFile);
                    return new FileUploadResponse() { IsSuccess = true, ResultMsg = "上传成功", File = commonFile };
                }
                else
                {
                    return new FileUploadResponse() { IsSuccess = false, ResultMsg = "上传失败，未检测上传的文件信息~" };
                }

            }
            catch (Exception ex)
            {
                return new FileUploadResponse() { IsSuccess = false, ResultMsg = "文件保存失败，异常信息为：" + ex.Message };
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<DataAccessResult> PersistenceFile([FromQuery] Guid id)
        {
            var res = await _fileService.LazyUpdateAsync(x => x.Id == id, new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(nameof(SystemCommonFile.IsTemp), false)
            });

            if (res.IsSuccess)
            {
                return DataAccessResult.Success("持久化成功");
            }
            else
            {
                return DataAccessResult.Success("持久化失败");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<DataAccessResult> DeleteFile(Guid id)
        {
            var file = await _fileService.GetApiBoAsync(x => x.Id == id && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier);
            if (file != null)
            {
                if (await _fileRepository.DeleteFileAsync(file.FilePath!))
                {
                    return await _fileService.DeleteBoAsync(x => x.Id == id);
                }
                return DataAccessResult.Error("文件成功删除，数据删除失败");
            }
            else
            {
                return DataAccessResult.Error("文件不存在");
            }
        }

        private async Task _UpdateLastAccessTime(Guid fileId)
        {
            await _fileService.LazyUpdateAsync(x => x.Id == fileId,
                new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>(nameof(SystemCommonFile.LastAccessTime), DateTime.Now)
                });
        }


        private async Task<ApplicationUserDQM?> _GetApplicationUser()
        {
            try
            {
                var userId = HttpContext.User.Identity.GetSubjectId();

                var user = await _applicationUserService.GetApiBoAsync(x => x.Id == Guid.Parse(userId) && x.IsEnable);

                if (user == null)
                {
                    user!.Id = Guid.Empty;
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> _SaveFileAsync(IFormFile file, string filePath)
        {
            var stream = file.OpenReadStream();
            //文件保存
            return await _fileRepository.SaveFileAsync(stream, filePath);
        }
    }
}

using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace LightPoint.IdentityServer.ServerInfrastructure.FileStorage
{
    public class FileRepositoryImpByMinIO : IFileRepository
    {
        private readonly IMinioClientFactory _minioClientFactory;
        private readonly ILogger<FileRepositoryImpByMinIO> _logger;
        private readonly IMinIOConfigsProvider _minIOConfigsProvider;

        public FileRepositoryImpByMinIO(IMinioClientFactory minioClientFactory,
            ILogger<FileRepositoryImpByMinIO> logger,
            IMinIOConfigsProvider minIOConfigsProvider)
        {
            _minioClientFactory = minioClientFactory;
            _logger = logger;
            _minIOConfigsProvider = minIOConfigsProvider;
        }

        public virtual async Task<Stream?> ReadFileAsync(string fileName)
        {
            var minioClient = _minioClientFactory.CreateClient();
            if (_minIOConfigsProvider.WithSSL)
            {
                minioClient = minioClient.WithSSL();
            }
            minioClient = minioClient.Build();
            MemoryStream stream = new MemoryStream();
            _logger.LogInformation("MinIO：读取图片" + _minIOConfigsProvider.Bucket + "--" + fileName);
            var args = new GetObjectArgs().WithBucket(_minIOConfigsProvider.Bucket).WithObject(fileName)
                            .WithCallbackStream((cb) =>
                            {
                                cb.CopyTo(stream);
                            });
            await minioClient.GetObjectAsync(args);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public virtual async Task<bool> SaveFileAsync(Stream file, string filePath)
        {
            var minioClient = _minioClientFactory.CreateClient();
            if (_minIOConfigsProvider.WithSSL)
            {
                minioClient = minioClient.WithSSL();
            }
            minioClient = minioClient.Build();
            try
            {
                var putObjectArgs = new PutObjectArgs()
                .WithBucket(_minIOConfigsProvider.Bucket)
                .WithObject(filePath)
                .WithObjectSize(file.Length)
                .WithStreamData(file);
                await minioClient.PutObjectAsync(putObjectArgs);

                file.Close();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("MinIO文件上传失败:" + ex.Message);
                return false;
            }

        }


        public virtual async Task<string> GetFilePathAsync(string filePath)
        {
            var minioClient = _minioClientFactory.CreateClient();
            if (_minIOConfigsProvider.WithSSL)
            {
                minioClient = minioClient.WithSSL();
            }
            minioClient = minioClient.Build();
            try
            {
                string sharedUrl = await minioClient.PresignedGetObjectAsync(
                    new PresignedGetObjectArgs().WithBucket(_minIOConfigsProvider.Bucket).WithExpiry(_minIOConfigsProvider.SharedUrlExpireSecends).WithObject(filePath)
            );
                return sharedUrl;
            }
            catch (Exception)
            {
                return "";
            }

        }

        public virtual async Task<bool> DeleteFileAsync(string filePath)
        {
            var minioClient = _minioClientFactory.CreateClient();
            if (_minIOConfigsProvider.WithSSL)
            {
                minioClient = minioClient.WithSSL();
            }
            minioClient = minioClient.Build();
            var removeObjectArgs = new RemoveObjectArgs().WithObject(filePath).WithBucket(_minIOConfigsProvider.Bucket);
            try
            {
                await minioClient.RemoveObjectAsync(removeObjectArgs);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("MinIO文件删除失败:" + ex.Message);
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.FileStorage
{
    public interface IFileRepository
    {
        Task<Stream?> ReadFileAsync(string fileName);
        Task<bool> SaveFileAsync(Stream file, string filePath);
        Task<bool> DeleteFileAsync(string filePath);
        Task<string> GetFilePathAsync(string filePath);
    }
}

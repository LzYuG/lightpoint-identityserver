using LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.FileStorage.Tools
{
    /// <summary>
    /// 图片处理辅助方法
    /// 1. 检查文件后缀
    /// </summary>
    public static class FileHandle
    {
        public static FileExtensionEnum? CheckFileExtensionNames(List<FileExtensionEnum> allowTypes, Stream file)
        {
            BinaryReader r = new BinaryReader(file);
            string bx = " ";
            byte buffer;
            try
            {
                buffer = r.ReadByte();
                bx = buffer.ToString();
                buffer = r.ReadByte();
                bx += buffer.ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
            r.Close();
            file.Close();
            return allowTypes.FirstOrDefault(x => ((int)x).ToString() == bx);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources
{
    public enum FileExtensionEnum : long
    {
        JPG = 255216,
        GIF = 7173,
        BMP = 6677,
        PNG = 13780,
        COM = 7790,
        EXE = 7790,
        DLL = 7790,
        RAR = 8297,
        ZIP = 8075,
        XML = 6063,
        HTML = 6033,
        ASPX = 239187,
        CS = 117115,
        JS = 119105,
        TXT = 210187,
        SQL = 255254,
        BAT = 64101,
        BTSEED = 10056,
        RDP = 255254,
        PSD = 5666,
        PDF = 3780,
        CHM = 7384,
        LOG = 70105,
        REG = 8269,
        HLP = 6395,
        DOC = 208207,
        XLS = 208207,
        DOCX = 8075,
        XLSX = 8075,
        MP4 = 0x00000020667479706D70,
        MKV = 0x1A45DFA3,
        AVI = 0x52494646,
        MOV = 0x6D6F6F76,
        FLV = 0x464C5601, // FLV文件
        RMVB = 0x2E524D46, // RMVB文件
        MPEG = 0x000001BA, // MPEG文件
        // 请注意，以上魔数可能并不完全准确，具体魔数可能会因文件的不同而有所变化
    }
}

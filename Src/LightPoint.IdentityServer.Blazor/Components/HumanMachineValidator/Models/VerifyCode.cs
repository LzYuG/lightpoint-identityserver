namespace LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator.Models
{
    public class VerifyCode
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string? Result { get; set; }

        /// <summary>
        /// 验证码文件流
        /// </summary>
        public byte[]? Image { get; set; }
    }
}

namespace LightPoint.IdentityServer.ServerInfrastructure.Email;
public class ValidationCodeEmailModel
{
    /// <summary>
    /// 发送给
    /// </summary>
    public List<string> To { get; }
    /// <summary>
    /// 验证码
    /// </summary>
    public string? Code { get; set; }
    /// <summary>
    /// 客户端名称
    /// </summary>
    public string? ClientName { get; set; }
    /// <summary>
    /// 验证码申请的说明
    /// </summary>
    public string? OpertionType { get; set; }

    public string? RemoteIP { get; set; }

    public ValidationCodeEmailModel(List<string> to)
    {
        To = to;
    }
}

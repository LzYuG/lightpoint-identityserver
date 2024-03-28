using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using Microsoft.AspNetCore.Hosting;
using LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoModels.DM04.LogAuditingResources;

namespace LightPoint.IdentityServer.ServerInfrastructure.Email;
public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly IAppService<Guid, ServerEmailConfig, ServerEmailConfigDQM, ServerEmailConfigDCM> _service;
    private readonly IAppService<Guid, ServerRunningLog, ServerRunningLogDM, ServerRunningLogDM> _serverRunningLogService;
    private readonly IConfidentialService _confidentialService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SmtpEmailService(ILogger<SmtpEmailService> logger, IAppService<Guid, ServerEmailConfig, ServerEmailConfigDQM, ServerEmailConfigDCM> service,
        IAppService<Guid, ServerRunningLog, ServerRunningLogDM, ServerRunningLogDM> serverRunningLogService,
        IConfidentialService confidentialService, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _service = service;
        _serverRunningLogService = serverRunningLogService;
        _confidentialService = confidentialService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task SendValidationCodeAsync(TenantInfo tenantInfo, ValidationCodeEmailModel model, CancellationToken cancellationToken)
    {
        try
        {
            var config = await _service.GetApiBoAsync(x => x.IsEnable && x.TenantIdentifier == tenantInfo.TenantIdentifier);

            if (config == null)
            {
                throw new InvalidOperationException("该租户未配置邮件发送配置");
            }

            config = await _confidentialService.DecryptModelConfidentialPropsAsync(config);

            var content = "";
            if (string.IsNullOrWhiteSpace(config.ValidationCodeTemplate))
            {
                content = (_webHostEnvironment.WebRootPath + config.ValidationCodeTemplatePath).Replace("\\", "/");
                var htmlBody = File.ReadAllText(content);

                htmlBody = htmlBody.Replace("{{CLIENTNAME}}", config.EmailClientName);
                htmlBody = htmlBody.Replace("{{OPERATIONTYPE}}", model.OpertionType);
                htmlBody = htmlBody.Replace("{{TOKEN}}", model.Code);

                content = htmlBody;
            }

            var email = new MimeMessage();

            // From
            email.From.Add(new MailboxAddress(config.FromName, config.From));

            // To
            foreach (string address in model.To)
                email.To.Add(MailboxAddress.Parse(address));

            // Reply To
            if (!string.IsNullOrEmpty(config.ReplyTo))
                email.ReplyTo.Add(new MailboxAddress(config.ReplyToName, config.ReplyTo));

            // Content
            var builder = new BodyBuilder();
            email.Sender = new MailboxAddress(config.FromName, config.From);
            email.Subject = config.ValidationCodeSubject;
            builder.HtmlBody = content;

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(config.SMTPServerHost, config.SMTPServerPort, SecureSocketOptions.SslOnConnect, cancellationToken);
            await smtp.AuthenticateAsync(config.Account, config.Password, cancellationToken);
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            await _serverRunningLogService.SetAndSaveEntityData(new ServerRunningLogDM()
            {
                CreateTime = DateTime.Now,
                Id = Guid.NewGuid(),
                Remark = "邮件发送错误",
                ServerRunningLogType = Shared.BusinessEnums.BE04.LogAuditingResources.ServerRunningLogType.服务异常,
                TenantIdentifier = tenantInfo.TenantIdentifier,
                Errors = ex.Message + "；异常位置：" + ex.StackTrace,
                RemoteIP = model.RemoteIP
            }, true);
            _logger.LogError(ex, ex.Message);
        }
    }

}

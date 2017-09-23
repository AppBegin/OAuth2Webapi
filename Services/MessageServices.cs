using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysms.Model.V20170525;
using Aliyun.Acs.Dysms.Transform.V20170525;

namespace auth
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly ILogger<AuthMessageSender> _logger;

        string accesskey,secret,signname,templatecode;

        public AuthMessageSender(ILogger<AuthMessageSender> logger,IConfiguration configuration)
        {
            _logger = logger;
            accesskey = configuration.GetConnectionString("accesskey");
            secret = configuration.GetConnectionString("secret");
            signname = configuration.GetConnectionString("signname");
            templatecode = configuration.GetConnectionString("templatecode");

        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            _logger.LogInformation("Email: {email}, Subject: {subject}, Message: {message}", email, subject, message);
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string phone, string code)
        {
            // Plug in your SMS service here to send a text message.
            string product = "Dysmsapi";
            string domain = "dysmsapi.aliyuncs.com";
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accesskey, secret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try {
                request.SignName = signname;
                request.TemplateCode = templatecode;
                request.PhoneNumbers = phone;
                request.TemplateParam = "{\"number\":\""+code+"\"}"; //"123456";
                SendSmsResponse httpResponse = acsClient.GetAcsResponse(request);
            } catch (ServerException e) {
                _logger.LogInformation("ServerException:"+e.ErrorMessage);
            }
            catch (ClientException e) {
                _logger.LogInformation("ClientException:"+e.ErrorMessage);
            }
            _logger.LogInformation("SMS: {number}, Message: {message}", phone, code);
            return Task.FromResult(0);
        }
    }
}

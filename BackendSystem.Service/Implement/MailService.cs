using BackendSystem.Service.Interface;
using BackendSystem.Service.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BackendSystem.Service.Implement
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;    
        private readonly ITokenService _tokenManager;
        private readonly ILogger<MailService> _logger;

        public MailService(IConfiguration configuration, ITokenService tokenManager,ILogger<MailService> logger)
        {
            _configuration = configuration;
            _tokenManager = tokenManager;
            _logger = logger;
        }
        public async Task SendRegisterEamil(string toEmail,string name, int Id, string role)
        {
            try
            {
                _logger.LogInformation($"Sending email to {toEmail} for user {name}, ID: {Id}");

                using var smtpClient = new SmtpClient(_configuration["Email:Smtp:Host"])
                {
                    Port = int.Parse(_configuration["Email:Smtp:Port"]),
                    Credentials = new NetworkCredential(_configuration["Email:Smtp:Username"], _configuration["Email:Smtp:Password"]),
                    EnableSsl = true,
                };

                User user = new User()
                {
                    Id = Id,
                    Name = name,
                    Email = toEmail,
                    Role = role
                };

                var base64Token = GenerateTokenForEmailVerification(user);
                var domain = _configuration["Domain:Frontend:Deploy"];
                var verificationLink = $"{domain}/verifyEmail?token={WebUtility.UrlEncode(base64Token)}&email={WebUtility.UrlEncode(user.Email)}";
                var body = $"<p>Thank you for registering. Please click <a href='{verificationLink}'>here</a> to verify your email.</p>";


                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:From"]),
                    Subject = "Please verify your email",
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation($"Email sent successfully to {toEmail}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {toEmail} for user {name}, ID: {Id}");
                throw;
            }            
        }

        private string GenerateTokenForEmailVerification(User user)
        {
            var token = _tokenManager.GenerateToken(user);
            var tokenJson = JsonConvert.SerializeObject(token);
            var tokenBytes = Encoding.UTF8.GetBytes(tokenJson);
            return Convert.ToBase64String(tokenBytes);
        }


    }
}

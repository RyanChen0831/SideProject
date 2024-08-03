using BackendSystem.Service.Interface;
using BackendSystem.Service.Security;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;

namespace BackendSystem.Service.Implement
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;    
        private readonly ITokenService _tokenManager;
        public MailService(IConfiguration configuration, ITokenService tokenManager)
        {
            _configuration = configuration;
            _tokenManager = tokenManager;
        }
        public async void SendRegisterEamil(string toEmail,string name, int Id, string role)
        {
            var smtpClient = new SmtpClient(_configuration["Email:Smtp:Host"])
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
                Role= role
            };

            var token = _tokenManager.GenerateToken(user);
            var tokenJson = JsonConvert.SerializeObject(token);
            var tokenBytes = Encoding.UTF8.GetBytes(tokenJson);
            var base64Token = Convert.ToBase64String(tokenBytes);
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
        }

        
    }
}

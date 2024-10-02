using Ecommerce.Core.IRepositories.IServices;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendMailAsync(string ToEmail, string Subject, string Message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("mahmoudEmail" , configuration.GetSection("EmailSettings")["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("client" , ToEmail));
            emailMessage.Subject = Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = Message };

            var client = new SmtpClient();
            await client.ConnectAsync(configuration.GetSection("EmailSettings")["SmtpServer"],
                int.Parse(configuration.GetSection("EmailSettings")["Port"]),
                bool.Parse(configuration.GetSection("EmailSettings")["SslUse"]));

            await client.AuthenticateAsync(configuration.GetSection("EmailSettings")["SenderEmail"] ,
                configuration.GetSection("EmailSettings")["Password"]);

            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}

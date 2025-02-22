using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TaskMgtMVC.Services
{
    public class EmailServices
    {
        private readonly string _smtpServer = "smtp.gmail.com"; 
        private readonly int _smtpPort = 587; 
        private readonly string _emailFrom = "laharipotta7@gmail.com"; 
        private readonly string _emailPassword = "jpwz tqdg glrw gbhz";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Task Management System", _emailFrom));
            email.To.Add(new MailboxAddress("User", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailFrom, _emailPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
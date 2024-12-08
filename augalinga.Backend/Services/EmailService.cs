using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace augalinga.Backend.Services
{
    public class EmailService : IEmailService
    {
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SenderEmail = "applicationaugalinga@gmail.com"; 
        private const string SenderPassword = "svts hper qscs wtxf"; 

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(SenderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false, // Set to true if you're using HTML in the body
            };
            mailMessage.To.Add(recipientEmail);

            // Set up SMTP client
            var smtpClient = new SmtpClient
            {
                Host = SmtpHost,
                Port = SmtpPort,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword)
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }
    }
}

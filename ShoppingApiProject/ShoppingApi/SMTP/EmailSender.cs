using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingApi.SMTP
{
    public class EmailSender : IEmailSender
    {
        public void SendEmailAsync(string email, string subject, string message)
        {
            var username = "tegaumurhurhu@gmail.com";
            var password = "zviukwzyvuhwsoas";
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(username);
            mailMessage.Subject = subject;
            mailMessage.To.Add(new MailAddress(email));
            mailMessage.Body = $"Here is code please enter it, for authentication purpose {message}. Thank you";
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

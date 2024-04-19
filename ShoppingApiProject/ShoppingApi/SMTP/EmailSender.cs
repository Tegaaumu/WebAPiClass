using System.Net.Mail;
using System.Net;

namespace ShoppingApi.SMTP
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("tegaumurhurhu@gmail.com", "zviukwzyvuhwsoas")
            };
            client.EnableSsl = true;

            return client.SendMailAsync(
                new MailMessage(from: "your.email@live.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}

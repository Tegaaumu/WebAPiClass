namespace ShoppingApi.SMTP
{
    public interface IEmailSender
    {
        void SendEmailAsync(string email, string subject, string message);
    }
}

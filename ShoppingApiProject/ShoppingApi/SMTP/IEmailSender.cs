﻿namespace ShoppingApi.SMTP
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
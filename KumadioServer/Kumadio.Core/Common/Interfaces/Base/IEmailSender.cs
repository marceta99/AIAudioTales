namespace Kumadio.Core.Common.Interfaces.Base
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}

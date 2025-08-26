namespace EF_Core.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink);
    }
}

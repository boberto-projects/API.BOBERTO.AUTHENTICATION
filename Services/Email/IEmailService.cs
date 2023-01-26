namespace api_authentication_boberto.Services.Email
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }
}

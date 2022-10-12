namespace api_authentication_boberto.Services.Interfaces
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }
}

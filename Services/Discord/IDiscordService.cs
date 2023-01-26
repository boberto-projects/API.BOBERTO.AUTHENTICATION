namespace api_authentication_boberto.Services.Discord
{
    public interface IDiscordService
    {
        Task SendCode(string code);
        Task SendMessage(string message);
    }
}

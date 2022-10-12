using api_authentication_boberto.Integrations.Zenvia.Request;
using api_authentication_boberto.Integrations.Zenvia.Response;
using RestEase;

namespace api_authentication_boberto.Integrations.Zenvia
{
    [Header("User-Agent", "ApiBobertoAuth")]
    [Header("Cache-Control", "no-cache")]
    [Header("Content-Type", "application/json")]
    public interface IZenviaApi
    {
        [Header("X-API-TOKEN")]
        string ApiKey { get; set; }

        [Post("channels/sms/messages")]
        Task<SendSMSResponse> EnviarSMS([Body] SendSMSRequest request);
    }
}

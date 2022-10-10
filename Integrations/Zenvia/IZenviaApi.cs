using api_authentication_boberto.Integrations.Zenvia.Request;
using api_authentication_boberto.Integrations.Zenvia.Response;
using RestEase;

namespace api_authentication_boberto.Integrations.Zenvia
{
    public interface IZenviaApi
    {
        [Post("channels/sms/messages")]
        Task<SendSMSResponse> EnviarSMS([Body] SendSMSRequest request);
    }
}

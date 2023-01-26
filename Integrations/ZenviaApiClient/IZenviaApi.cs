using RestEase;

namespace api_authentication_boberto.Integrations.ZenviaApiClient
{
    public interface IZenviaApi
    {
        [Header("X-API-TOKEN")]
        string ApiKey { get; set; }

        [Post("channels/sms/messages")]
        Task<SendSMSResponse> SendSMS([Body] SendSMSRequest request);
    }
}

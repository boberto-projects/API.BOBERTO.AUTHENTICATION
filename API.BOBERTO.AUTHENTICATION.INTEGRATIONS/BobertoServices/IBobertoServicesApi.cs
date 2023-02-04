using RestEase;

namespace API.BOBERTO.AUTHENTICATION.INTEGRATIONS.BobertoServices
{
    public interface IBobertoServicesApi
    {
        [Header("ApiKey")]
        string ApiKey { get; set; }
    }
}

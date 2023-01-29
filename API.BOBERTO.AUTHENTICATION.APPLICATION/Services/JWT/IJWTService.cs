using API.BOBERTO.AUTHENTICATION.DOMAIN.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.JWT
{
    public interface IJWTService
    {
        bool Validate(string token);
        string Generate(UserModel user, DateTime? expiration = null);
    }
}

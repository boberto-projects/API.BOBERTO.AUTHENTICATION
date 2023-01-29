using api_authentication_boberto.Domain.CustomDbContext;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.JWT
{
    public interface IJWTService
    {
        bool Validate(string token);
        string Generate(UserModel user, DateTime? expiration = null);
    }
}

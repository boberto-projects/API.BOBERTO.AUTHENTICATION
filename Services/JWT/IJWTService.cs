using api_authentication_boberto.Domain.CustomDbContext;

namespace api_authentication_boberto.Services.JWT
{
    public interface IJWTService
    {
        bool Validate(string token);
        string Generate(UserModel user, DateTime? expiration = null);
    }
}

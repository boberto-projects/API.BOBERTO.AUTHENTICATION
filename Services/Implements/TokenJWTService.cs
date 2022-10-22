using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Models.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_authentication_boberto.Services.Implements
{
    public class TokenJWTService
    {
        private JwtConfig _jwtConfig;
        public TokenJWTService(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public string GerarTokenJWT(UsuarioModel usuario, DateTime? expriacao = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", usuario.UsuarioId.ToString()),
                }),
                Expires = expriacao.GetValueOrDefault(DateTime.UtcNow.AddHours(1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

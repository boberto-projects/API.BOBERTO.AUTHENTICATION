using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Models.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_authentication_boberto.Services.JWT
{
    public class JWTService : IJWTService
    {
        private JwtConfig _jwtConfig;
        public JWTService(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
            JWTKey = Encoding.UTF8.GetBytes(jwtConfig.Value.Key);
        }

        public string Generate(UserModel usuario, DateTime? expiracao = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", usuario.UserId.ToString()),
                }),
                Expires = expiracao.GetValueOrDefault(DateTime.UtcNow.AddHours(1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(JWTKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public bool Validate(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private byte[] JWTKey { get; set; }
        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = _jwtConfig.Issuer,
                ValidAudience = _jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(JWTKey) // The same key as the one that generate the token
            };
        }
    }
}

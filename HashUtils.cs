using Microsoft.IdentityModel.Tokens;
using NuGet.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_authentication_boberto
{
    public static class HashUtils
    {
        //public static string GerarTokenJWT(IConfiguration _config)
        //{
        //    var issuer = _config["Jwt:Issuer"];
        //    var audience = _config["Jwt:Audience"];
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(issuer: issuer, audience: audience,
        //    expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var stringToken = tokenHandler.WriteToken(token);
        //    return stringToken;
        //}

        public static string GerarTokenJWT(this UsuarioModel usuario, IConfiguration _config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Nome),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

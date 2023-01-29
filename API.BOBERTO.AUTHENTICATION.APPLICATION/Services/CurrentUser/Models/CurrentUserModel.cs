using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.Authentication;
using api_authentication_boberto.Models.Enums;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string NumeroCelular { get; set; }
        public string Nome { get; set; }

        public bool UsarEmail { get; set; }
        public bool UsarNumeroCelular { get; set; }
        public RolesEnum Role { get; set; }
    }


}

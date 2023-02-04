using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.Authentication;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }

        public bool EmailEnabled { get; set; }
        public bool PhoneNumberEnabled { get; set; }
        public RolesEnum Role { get; set; }
    }


}

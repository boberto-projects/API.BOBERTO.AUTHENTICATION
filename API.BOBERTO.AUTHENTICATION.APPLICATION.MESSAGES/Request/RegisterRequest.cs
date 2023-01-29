using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public void Valid()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Email and password are required");
            }
        }
    }
}

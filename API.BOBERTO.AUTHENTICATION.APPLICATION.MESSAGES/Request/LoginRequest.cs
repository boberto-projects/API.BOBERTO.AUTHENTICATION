
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Code { get; set; }
        public void Validate()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Wrong email or password");
            }
        }

    }
}

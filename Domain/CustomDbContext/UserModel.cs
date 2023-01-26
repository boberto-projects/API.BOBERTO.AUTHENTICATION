using api_authentication_boberto.Models.Enums;

namespace api_authentication_boberto.Domain.CustomDbContext
{
    public class UserModel
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime LastLogin { get; set; }

        public int UserConfigId { get; set; }
        public virtual UserConfigModel UserConfig { get; set; }
        public ICollection<ApiKeyModel> ApiKeys { get; set; }
        public RolesEnum Role { get; set; }
        public UserModel()
        {
            ApiKeys = new List<ApiKeyModel>();
        }

    }
}

namespace api_authentication_boberto.Models.Cache
{
    public class UserCacheModel
    {
        public DateTime? LastAttempt { get; set; }
        public DateTime LastLogin { get; set; }
        public int LoginAttempts { get; set; }
        public string? Email { get; set; }
        public int? UserId { get; set; }
        public bool Blocked { get; set; }

        public UserCacheModel()
        {

        }
    }
}

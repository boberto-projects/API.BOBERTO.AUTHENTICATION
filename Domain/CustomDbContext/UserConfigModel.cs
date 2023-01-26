namespace api_authentication_boberto.Domain.CustomDbContext
{
    public class UserConfigModel
    {
        public int UserConfigId { get; set; }
        public bool EnabledEmail { get; set; }
        public bool EnabledPhoneNumber { get; set; }

        //public virtual UsuarioModel Usuario { get; set; }
    }
}

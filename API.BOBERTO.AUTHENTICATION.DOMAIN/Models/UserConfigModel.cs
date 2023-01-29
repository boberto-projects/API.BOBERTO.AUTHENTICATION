namespace API.BOBERTO.AUTHENTICATION.DOMAIN.Models
{
    public class UserConfigModel
    {
        public int UserConfigId { get; set; }
        public bool EnabledEmail { get; set; }
        public bool EnabledPhoneNumber { get; set; }

        //public virtual UsuarioModel Usuario { get; set; }
    }
}

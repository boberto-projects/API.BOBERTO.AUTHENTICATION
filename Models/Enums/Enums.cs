namespace api_authentication_boberto.Models
{
    public enum StatusCodeEnum
    {
        Validacao = 405,
        Negocio = 400,
        NaoEncontrado = 404,
        NaoAutorizado = 401,
        Interno = 500,
    }
    public enum CodigoOTPEnum
    {
        CodigoOTPInvalido = 1,
        CodigoOTPNaoInformado = 2,
    }
}

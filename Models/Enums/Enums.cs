namespace api_authentication_boberto.Models
{
    public enum StatusCodeEnum
    {
        VALIDATION = 405,
        BUSINESS = 400,
        NOTFOUND = 404,
        NOTAUTHORIZED = 401,
        INTERN = 500,
    }
    public enum OTPEnum
    {
        OTPInvalid = 1,
        OTPNOTINFORMED = 2,
    }
    public enum ScopesEnum
    {
        SERVER_CREATE,
        SERVER_DELETE,
        SERVER_UPDATE,
        SERVER_EDIT,
        MODPACK_UPDATE,
        MODPACK_CREATE,
        MODPACK_EDIT,
        MODPACK_DELETE
    }
    public enum RolesEnum
    {
        USER,
        MODPACK_CREATOR,
        SERVER_MANAGER,
        ADMINISTRATOR
    }
}

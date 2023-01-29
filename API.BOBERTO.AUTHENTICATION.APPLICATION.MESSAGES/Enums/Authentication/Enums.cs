using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.Authentication
{
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

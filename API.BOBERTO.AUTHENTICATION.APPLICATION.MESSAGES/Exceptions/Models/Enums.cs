using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models
{
    public enum StatusCodeEnum
    {
        VALIDATION = 405,
        BUSINESS = 400,
        NOTFOUND = 404,
        NOTAUTHORIZED = 401,
        INTERN = 500,
    }

    public enum ExceptionTypeEnum
    {
        UNKNOWN = 0,
        AUTHORIZATION,
        INTERNAL
    }
}

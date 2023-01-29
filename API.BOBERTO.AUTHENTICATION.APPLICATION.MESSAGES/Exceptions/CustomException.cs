using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models.Serialize;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions
{
    [Serializable]
    public class CustomException : Exception
    {
        public int StatusCode { get; }
        public StatusCodeEnum Type { get; }

        public CustomException(StatusCodeEnum statusCode, string message)
       : base(message)
        {
            Type = statusCode;
            StatusCode = StatusCode;
        }

        public CustomExceptionResponse ObterResponse()
        {
            var response = new CustomExceptionResponse()
            {
                Type = GetType(),
                Message = Message
            };

            return response;
        }

        private string GetType()
        {
            switch (Type)
            {
                case StatusCodeEnum.BUSINESS:
                    return "business";
                case StatusCodeEnum.NOTAUTHORIZED:
                    return "not_authorized";
                case StatusCodeEnum.VALIDATION:
                    return "validation";

            }
            return "not_recognized";
        }
    }
}

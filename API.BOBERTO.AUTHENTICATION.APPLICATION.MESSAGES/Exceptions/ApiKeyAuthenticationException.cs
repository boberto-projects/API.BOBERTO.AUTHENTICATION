﻿using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models.Serialize;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions
{
    [Serializable]
    public class ApiKeyAuthenticationException : Exception
    {
        public ExceptionTypeEnum Type { get; }
        public StatusCodeEnum StatusCode { get; }
        public string Message { get; set; }
        public ApiKeyAuthenticationException(ExceptionTypeEnum type)
        {
            Type = type;
            StatusCode = GetStatusCode();
            Message = GetMessage();
        }

        public ApiKeyExceptionResponse GetResponse()
        {
            return new ApiKeyExceptionResponse
            {
                Message = Message,
                StatusCode = (int)StatusCode,
                Type = GetType()

            };
        }
        public string GetMessage()
        {
            switch (Type)
            {
                case ExceptionTypeEnum.AUTHORIZATION:
                    return "Api Key invalid or expired.";

                default:
                    return "message not handled";
            }
        }
        public StatusCodeEnum GetStatusCode()
        {
            switch (Type)
            {
                case ExceptionTypeEnum.AUTHORIZATION:
                    return StatusCodeEnum.NOTAUTHORIZED;

                default:
                    return StatusCodeEnum.INTERN;
            }
        }
        private string GetType()
        {
            switch (StatusCode)
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

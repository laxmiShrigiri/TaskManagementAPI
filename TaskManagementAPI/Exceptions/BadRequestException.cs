using System.Net;

namespace TaskManagementAPI.Exceptions;

public sealed class BadRequestException(string message)
    : AppException(message, HttpStatusCode.BadRequest);
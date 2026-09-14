using System.Net;

namespace TaskManagementAPI.Exceptions;

public sealed class NotFoundException(string message)
    : AppException(message, HttpStatusCode.NotFound);
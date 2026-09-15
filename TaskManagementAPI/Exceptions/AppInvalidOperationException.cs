using System.Net;

namespace TaskManagementAPI.Exceptions;

public sealed class AppInvalidOperationException(string message)
    : AppException(message, HttpStatusCode.BadRequest);
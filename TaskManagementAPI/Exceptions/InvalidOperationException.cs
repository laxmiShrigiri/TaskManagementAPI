using System.Net;

namespace TaskManagementAPI.Exceptions;

public sealed class InvalidOperationException(string message)
    : AppException(message, HttpStatusCode.NotFound);

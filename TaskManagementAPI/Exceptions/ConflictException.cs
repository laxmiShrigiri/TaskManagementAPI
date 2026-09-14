using System.Net;

namespace TaskManagementAPI.Exceptions;

public sealed class ConflictException(string message)
    : AppException(message, HttpStatusCode.Conflict);
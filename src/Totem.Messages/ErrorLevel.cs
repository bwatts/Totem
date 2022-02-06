using System.Net;

namespace Totem;

public enum ErrorLevel
{
    BadRequest = HttpStatusCode.BadRequest,
    Unauthorized = HttpStatusCode.Unauthorized,
    Forbidden = HttpStatusCode.Forbidden,
    NotFound = HttpStatusCode.NotFound,
    NotAllowed = HttpStatusCode.MethodNotAllowed,
    Conflict = HttpStatusCode.Conflict,
    Fatal = HttpStatusCode.InternalServerError
}

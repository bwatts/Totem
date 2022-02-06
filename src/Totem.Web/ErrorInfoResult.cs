using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Totem;

public class ErrorInfoResult : ObjectResult
{
    public ErrorInfoResult(IEnumerable<ErrorInfo> errors) : base(errors)
    {
        errors = errors?.ToArray() ?? throw new ArgumentNullException(nameof(errors));

        Value = errors;
        StatusCode = errors
            .Select(error => (int) error.Level)
            .DefaultIfEmpty((int) HttpStatusCode.InternalServerError)
            .Max();
    }
}

using System;

namespace Totem
{
    public class ErrorInfo
    {
        public ErrorInfo(string name, int code, ErrorLevel level = ErrorLevel.BadRequest)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException(nameof(name));

            Name = name;
            Code = code;
            Level = level;
        }

        public ErrorInfo(string name, ErrorLevel level = ErrorLevel.BadRequest)
          : this(name, (int) level, level)
        { }

        public ErrorInfo() : this("")
        { }

        public string Name { get; }
        public int Code { get; }
        public ErrorLevel Level { get; }

        public override string ToString() =>
            $"{Code} {Name} - {Level}";

        public static readonly ErrorInfo General = new("GeneralError");
        public static ErrorInfo BadRequest(string name) => new(name, ErrorLevel.BadRequest);
        public static ErrorInfo Unauthorized(string name) => new(name, ErrorLevel.Unauthorized);
        public static ErrorInfo Forbidden(string name) => new(name, ErrorLevel.Forbidden);
        public static ErrorInfo NotFound(string name) => new(name, ErrorLevel.NotFound);
        public static ErrorInfo Conflict(string name) => new(name, ErrorLevel.Conflict);
        public static ErrorInfo Fatal(string name) => new(name, ErrorLevel.Fatal);
    }
}
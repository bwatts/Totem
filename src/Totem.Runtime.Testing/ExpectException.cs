namespace Totem;

public class ExpectException : Exception
{
    public ExpectException()
    { }

    public ExpectException(string message) : base(message)
    { }

    public ExpectException(string message, Exception inner) : base(message, inner)
    { }
}

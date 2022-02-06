using System.Collections.Concurrent;

namespace Totem;

public class ErrorBag : ConcurrentBag<ErrorInfo>
{
    public ErrorBag() : base() { }
    public ErrorBag(IEnumerable<ErrorInfo> errors) : base(errors) { }

    public override string ToString() =>
        string.Join(Environment.NewLine, from error in this select error.ToString());

    public bool Any => !IsEmpty;

    public void ExpectNone()
    {
        if(Any)
        {
            throw new InvalidOperationException(ToString());
        }
    }
}

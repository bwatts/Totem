using Totem.Timeline;

namespace Acme.ProductImport
{
  public class ImportStarted : Event
  {
    public ImportStarted(string reason)
    {
      Reason = reason;
    }

    public readonly string Reason;
  }
}
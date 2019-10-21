using Totem.Timeline;

namespace Acme.ProductImport
{
  public class FinishImport : Command
  {
    public FinishImport(string reason)
    {
      Reason = reason;
    }

    public readonly string Reason;
  }
}
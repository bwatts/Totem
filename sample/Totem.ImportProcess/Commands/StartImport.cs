using Totem.Timeline;

namespace Acme.ProductImport
{
  public class StartImport : Command
  {
    public StartImport(string reason)
    {
      Reason = reason;
    }

    public readonly string Reason;
  }
}
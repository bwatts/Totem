using Totem.Timeline;

namespace Acme.ProductImport
{
  public class ImportFailed : Event
  {
    public string Error { get; }

    public ImportFailed(string error)
    {
      Error = error;
    }
  }
}
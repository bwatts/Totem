using Totem.Timeline;

namespace Acme.ProductImport.Queries
{
  public class ImportStatus : Query
  {
    public bool Importing;
    public string Reason;
    public string Error;

    void Given(ImportStarted e)
    {
      Importing = true;
      Reason = e.Reason;
      Error = null;
    }

    void Given(ImportFinished e)
    {
      Importing = false;
    }

    void Given(ImportFailed e)
    {
      Importing = false;
      Error = e.Error;
    }
  }
}
using Totem.Timeline;

namespace Acme.ProductImport.Topics
{
  public class ImportProcess : Topic
  {
    bool _importing;

    void Given(ImportStarted e)
    {
      _importing = true;
    }
      

    void Given(ImportFinished e)
    {
      _importing = false;
    }

    void When(StartImport e)
    {
      if (_importing)
      {
        Then(new ImportAlreadyStarted());
      }
      else
      {
        Then(new ImportStarted(e.Reason));
      }
    }

    void When(FinishImport e)
    {
      if (_importing)
      {
        Then(new ImportFinished());
      }
      else
      {
        Then(new ImportNotStarted());
      }
    }
  }
}
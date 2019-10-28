using Totem.Timeline;

namespace Acme.ProductImport.Topics
{
  public class FinancialProcess : Topic
  {
    int _balance;

    void Given(BalanceWithdrew e)
    {
      _balance = _balance - e.Amount;
    }

    void When(SetBalance e)
    {
      _balance = e.Balance;
    }
  }
}
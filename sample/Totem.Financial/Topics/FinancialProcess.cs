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

    void Given(BalanceSet e)
    {
      _balance = e.Balance;
    }

    void When(SetBalance e)
    {
      Then(new BalanceSet(e.Balance));
    }
  }
}
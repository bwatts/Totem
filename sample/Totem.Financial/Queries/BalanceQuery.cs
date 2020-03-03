using Totem.Timeline;

namespace Acme.ProductImport.Queries
{
  public class BalanceQuery : Query
  {
    public int Balance;

    void Given(BalanceWithdrew e)
    {
      Balance = Balance - e.Amount;
    }

    void Given(BalanceSet e)
    {
      Balance = e.Balance;
    }
  }
}
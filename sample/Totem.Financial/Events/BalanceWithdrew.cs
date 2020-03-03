using Totem.Timeline;

namespace Acme.ProductImport
{
  public class BalanceWithdrew : Event
  {
    public BalanceWithdrew(int amount)
    {
      Amount = amount;
    }

    public readonly int Amount;
  }
}
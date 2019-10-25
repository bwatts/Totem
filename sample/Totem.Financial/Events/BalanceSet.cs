using Totem.Timeline;

namespace Acme.ProductImport
{
  public class BalanceSet : Event
  {
    public BalanceSet(int balance)
    {
      Balance = balance;
    }

    public readonly int Balance;
  }
}
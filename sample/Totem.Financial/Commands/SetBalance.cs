using Totem.Timeline;

namespace Acme.ProductImport
{
  public class SetBalance : Command
  {
    public SetBalance(int balance)
    {
      Balance = balance;
    }

    public readonly int Balance;
  }
}
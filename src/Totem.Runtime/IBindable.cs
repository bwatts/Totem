namespace Totem.Runtime
{
  /// <summary>
  /// Describes an object hosting a set of bindable fields
  /// </summary>
  public interface IBindable
  {
    Fields Fields { get; }
  }
}
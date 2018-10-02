using System.Data;

namespace Totem.Data
{
  /// <summary>
	/// Describes an open ADO.NET connection to a relational database
	/// </summary>
  public interface IDbCall
  {
    IDbConnection Connection { get; }

    void Dispose(bool commit = false);
  }
}
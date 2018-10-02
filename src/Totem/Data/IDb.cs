using System;
using System.Data;
using System.Threading.Tasks;

namespace Totem.Data
{
  /// <summary>
  /// Describes a relational database accessed via ADO.NET
  /// </summary>
  public interface IDb
  {
    Task<bool> TestConnection();

    Task ExecuteAction(Func<IDbConnection, Task> action);

    Task<TResult> ExecuteFunc<TResult>(Func<IDbConnection, Task<TResult>> func);

    IDbCall StartCall();
  }
}
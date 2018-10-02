using System.Data;
using System.Transactions;

namespace Totem.Data.SqlClient
{
  /// <summary>
  /// An open connection to a SQL Server database
  /// </summary>
  public sealed class SqlDbCall : IDbCall
  {
    readonly TransactionScope _transaction;

    public SqlDbCall(TransactionScope transaction, IDbConnection connection)
    {
      _transaction = transaction;
      Connection = connection;
    }

    public IDbConnection Connection { get; }

    public void Dispose(bool commit = false)
    {
      if(commit)
      {
        _transaction.Complete();
      }

      _transaction.Dispose();

      Connection.Dispose();
    }
  }
}
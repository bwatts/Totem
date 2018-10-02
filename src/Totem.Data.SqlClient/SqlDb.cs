using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Totem.Data.SqlClient
{
  /// <summary>
  /// A SQL server database accessed via ADO.NET
  /// </summary>
  public sealed class SqlDb : IDb
  {
    readonly string _connectionString;

    public SqlDb(string connectionString)
    {
      _connectionString = connectionString;
    }

    public async Task<bool> TestConnection()
    {
      using(var connection = new SqlConnection(_connectionString))
      {
        await connection.OpenAsync();

        return connection.State == ConnectionState.Open;
      }
    }

    public async Task ExecuteAction(Func<IDbConnection, Task> action)
    {
      using(var transaction = StartTransaction())
      using(var connection = OpenConnection())
      {
        await action(connection);

        transaction.Complete();
      }
    }

    public async Task<TResult> ExecuteFunc<TResult>(Func<IDbConnection, Task<TResult>> func)
    {
      using(var transaction = StartTransaction())
      using(var connection = OpenConnection())
      {
        var result = await func(connection);

        transaction.Complete();

        return result;
      }
    }

    public IDbCall StartCall() =>
      new SqlDbCall(StartTransaction(), OpenConnection());

    TransactionScope StartTransaction()
    {
      // As specified here: http://blogs.msdn.com/b/dbrowne/archive/2010/06/03/using-new-transactionscope-considered-harmful.aspx

      var options = new TransactionOptions
      {
        IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
        Timeout = TransactionManager.MaximumTimeout
      };

      return new TransactionScope(TransactionScopeOption.Suppress, options, TransactionScopeAsyncFlowOption.Enabled);
    }

    IDbConnection OpenConnection()
    {
      var connection = new SqlConnection(_connectionString);

      connection.Open();

      return connection;
    }
  }
}
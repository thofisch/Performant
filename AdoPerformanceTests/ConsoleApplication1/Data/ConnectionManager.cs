using System.Data;
using ConsoleApplication1.Data.Impl;

namespace ConsoleApplication1.Data
{
	public class ConnectionManager : Disposable
	{
		private readonly IDatabase _database;
		private IDbConnection _connection;
		private ITransaction _transaction;

		public ConnectionManager(IDatabase database)
		{
			_database = database;
		}

		public ITransaction Transaction
		{
			get
			{
				if( _transaction==null )
				{
					_transaction = new TransactionImpl(this);
				}
				return _transaction;
			}
		}

		public IDbConnection GetConnection()
		{
			if( _connection==null )
			{
				_connection = _database.CreateConnection();
			}
			return _connection;
		}

		public ITransaction BeginTransaction()
		{
			return BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			Transaction.Begin(isolationLevel);
			return _transaction;
		}

		public void AfterTransaction()
		{
			_transaction = null;
		}

		public IDbCommand CreateCommand()
		{
			IDbCommand command = GetConnection().CreateCommand();
			command.CommandType = 0;
			Transaction.Enlist(command);
			return command;
		}

		protected override void DisposeManagedResources()
		{
			if( _transaction!=null )
			{
				_transaction.Dispose();
			}
			if( _connection!=null )
			{
				_connection.Dispose();
			}
		}
	}
}
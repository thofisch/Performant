using System;
using System.Data;

namespace ConsoleApplication1.Data.Impl
{
	public class TransactionImpl : Disposable, ITransaction
	{
		private readonly ConnectionManager _connectionManager;
		private IDbTransaction _transaction;
		private bool _begun;

		public TransactionImpl(ConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
		}

		public bool IsActive
		{
			get
			{
				return _begun && !WasRolledBack && !WasCommitted;
			}
		}

		public bool WasCommitted { get; private set; }

		public bool WasRolledBack { get; private set; }

		public void Begin()
		{
			Begin(IsolationLevel.Unspecified);
		}

		public void Begin(IsolationLevel isolationLevel)
		{
			if( _begun )
			{
				return;
			}

			try
			{
				_transaction = _connectionManager.GetConnection().BeginTransaction(isolationLevel);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Begin failed with SQL exception", exception);
			}

			_begun = true;
			WasCommitted = false;
			WasRolledBack = false;
		}

		public void Commit()
		{
			CheckValidTransaction();
			try
			{
				_transaction.Commit();
				WasCommitted = true;
				AfterTransactionCompletion();
				Dispose();
			}
			catch(Exception ex)
			{
				AfterTransactionCompletion();
				throw new InvalidOperationException("Commit failed with SQL exception", ex);
			}
		}

		private void CheckValidTransaction()
		{
			AssertNotDisposed();

			if( !_begun )
			{
				throw new InvalidOperationException("Transaction not successfully started");
			}
		}

		public void Enlist(IDbCommand command)
		{
			command.Transaction = _transaction;
		}

		public void Rollback()
		{
			CheckValidTransaction();

			try
			{
				_transaction.Rollback();
				WasRolledBack = true;
				Dispose();
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException("Rollback failed with SQL Exception", ex);
			}
			finally
			{
				AfterTransactionCompletion();
			}
		}

		private void AfterTransactionCompletion()
		{
			_connectionManager.AfterTransaction();
			_begun = false;
		}

		protected override void DisposeManagedResources()
		{
			if( _transaction!=null )
			{
				_transaction.Dispose();
			}
			if( IsActive )
			{
				AfterTransactionCompletion();
			}
		}
	}
}
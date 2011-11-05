using System;
using System.Data;

namespace ConsoleApplication1.Data
{
	public interface ITransaction : IDisposable
	{
		bool IsActive { get; }
		bool WasCommitted { get; }
		bool WasRolledBack { get; }

		void Begin();
		void Begin(IsolationLevel isolationLevel);
		void Commit();
		void Enlist(IDbCommand command);
		void Rollback();
	}
}
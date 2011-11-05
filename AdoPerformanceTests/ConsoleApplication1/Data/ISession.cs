using System;
using System.Collections.Generic;
using System.Data;

namespace ConsoleApplication1.Data
{
	public interface ISession : IDisposable
	{
		ITransaction BeginTransaction();
		ITransaction BeginTransaction(IsolationLevel isolationLevel);

		T Get<T>(object id);
		IList<T> FindAll<T>();

		void Create<T>(T instance);
		void Update<T>(T instance);
		void Delete<T>(object id);
	}
}
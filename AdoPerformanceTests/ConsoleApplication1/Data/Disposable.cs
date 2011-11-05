using System;

namespace ConsoleApplication1.Data
{
	public abstract class Disposable : IDisposable
	{
		private bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Disposable()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if( _disposed )
			{
				return;
			}

			if( disposing )
			{
				DisposeManagedResources();
			}

			DisposeUnmanagedResources();
			_disposed = true;
		}

		protected virtual void AssertNotDisposed()
		{
			if( _disposed )
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		protected abstract void DisposeManagedResources();

		protected virtual void DisposeUnmanagedResources()
		{
		}
	}
}
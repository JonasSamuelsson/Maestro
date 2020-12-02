using System;
using System.Collections.Concurrent;

namespace Maestro.Internals
{
	internal abstract class TransientDisposableTracker : IDisposable
	{
		public abstract void Add(IDisposable disposable);
		public abstract void Dispose();
	}

	internal abstract class TransientDisposableTracker<T> : TransientDisposableTracker
	{
		private readonly ConcurrentBag<T> _bag = new ConcurrentBag<T>();

		public override void Add(IDisposable disposable)
		{
			_bag.Add(CreateItem(disposable));
		}

		public override void Dispose()
		{
			foreach (var item in _bag)
			{
				try
				{
					DisposeItem(item);
				}
				catch
				{
					// ignored
				}
			}
		}

		protected abstract T CreateItem(IDisposable disposable);
		protected abstract void DisposeItem(T item);
	}
}
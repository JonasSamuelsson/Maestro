using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	internal class WeakRefTransientDisposableTracker : TransientDisposableTracker
	{
		private int _counter = 0;
		private readonly List<WeakReference<IDisposable>> _weakReferences = new List<WeakReference<IDisposable>>();

		public override void Add(IDisposable disposable)
		{
			lock (_weakReferences)
			{
				if (_counter++ > 1000)
				{
					_weakReferences
						.Where(x => !x.TryGetTarget(out _))
						.ToList()
						.ForEach(x => _weakReferences.Remove(x));

					_counter = 0;
				}

				_weakReferences.Add(new WeakReference<IDisposable>(disposable));
			}
		}

		public override void Dispose()
		{
			foreach (var weakReference in _weakReferences)
			{
				if (!weakReference.TryGetTarget(out var disposable))
					continue;

				try
				{
					disposable?.Dispose();
				}
				catch
				{
					// ignored
				}
			}
		}
	}
}
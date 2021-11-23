using System;

namespace Maestro.Internals
{
	internal abstract class TransientDisposableTracker : IDisposable
	{
		public abstract void Add(IDisposable disposable);
		public abstract void Dispose();
	}
}
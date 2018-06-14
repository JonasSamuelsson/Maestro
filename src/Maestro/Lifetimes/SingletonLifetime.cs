using Maestro.Utils;
using System;

namespace Maestro.Lifetimes
{
	internal class SingletonLifetime : Lifetime
	{
		private readonly object _lock = new object();
		private volatile object _instance;

		public override object Execute(Context context, Func<Context, object> factory)
		{
			if (_instance != null)
				return _instance;

			lock (_lock)
			{
				if (_instance != null)
					return _instance;

				var instance = factory.Invoke(context);

				if (instance is IDisposable disposable)
				{
					var proxy = new Disposable(() => disposable.Dispose());
					if (!context.ScopedContainer.RootScope.TryAdd(this, proxy))
						throw new InvalidOperationException();
				}

				_instance = instance;

				return instance;
			}
		}

		public override Lifetime MakeGeneric(Type[] genericArguments)
		{
			return new SingletonLifetime();
		}

		public override string ToString()
		{
			return "Singleton";
		}
	}
}
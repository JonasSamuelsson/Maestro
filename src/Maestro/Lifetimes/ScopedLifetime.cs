using System;

namespace Maestro.Lifetimes
{
	internal class ScopedLifetime : Lifetime
	{
		private readonly object _lock = new object();

		public override object Execute(Context context, Func<Context, object> factory)
		{
			var cache = context.CurrentScope.Cache;

			return cache.GetOrAdd(this, _ =>
			{
				lock (_lock)
				{
					return cache.TryGetValue(this, out var value)
						? value
						: factory.Invoke(context);
				}
			});
		}

		public override Lifetime MakeGeneric(Type[] genericArguments)
		{
			return new ScopedLifetime();
		}

		public override string ToString()
		{
			return "Scoped";
		}
	}
}
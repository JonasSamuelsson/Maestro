using Maestro.Internals;
using System;

namespace Maestro.Lifetimes
{
	internal class SingletonLifetime : ILifetime
	{
		public object Execute(Context context, Func<Context, object> factory)
		{
			var ctx = (Context)context;
			return ctx.Kernel.Root.InstanceCache.GetOrAdd(this, _ => new Lazy<object>(() => factory(context))).Value;
		}

		public override string ToString()
		{
			return "Singleton";
		}
	}
}
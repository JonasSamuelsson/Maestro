using System;

namespace Maestro.Lifetimes
{
	internal class ScopedLifetime : Lifetime
	{
		public override object Execute(Context context, Func<Context, object> factory)
		{
			var ctx = context;
			return ctx.Kernel.InstanceCache.GetOrAdd(this, _ => new Lazy<object>(() => factory(context))).Value;
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
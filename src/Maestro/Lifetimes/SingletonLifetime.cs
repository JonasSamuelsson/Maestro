using System;

namespace Maestro.Lifetimes
{
	internal class SingletonLifetime : Lifetime
	{
		public override object Execute(Context context, Func<Context, object> factory)
		{
			var ctx = context;
			return ctx.Kernel.Root.InstanceCache.GetOrAdd(this, _ => new Lazy<object>(() => factory(context))).Value;
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
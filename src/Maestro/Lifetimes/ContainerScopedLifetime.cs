using System;
using Maestro.Internals;

namespace Maestro.Lifetimes
{
	internal class ContainerScopedLifetime : ILifetime
	{
		public object Execute(IContext context, Func<IContext, object> factory)
		{
			var ctx = (Context)context;
			return ctx.Kernel.InstanceCache.GetOrAdd(this, _ => new Lazy<object>(() => factory(context))).Value;
		}

		public override string ToString()
		{
			return "Container scoped";
		}
	}
}
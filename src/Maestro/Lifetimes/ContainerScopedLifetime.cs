﻿using System;
using Maestro.Internals;

namespace Maestro.Lifetimes
{
	internal class ContainerScopedLifetime : ILifetime
	{
		public object Execute(Context context, Func<Context, object> factory)
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
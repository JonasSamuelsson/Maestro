using System;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class Context_tests
	{
		[Fact]
		public void Using_a_disposed_context_should_throw()
		{
			var context = new Context(string.Empty, new Kernel(new PluginLookup()));

			context.Dispose();

			object instance;
			Should.Throw<ObjectDisposedException>(() => ((IContext)context).TryGet(typeof(object), out instance));
			Should.Throw<ObjectDisposedException>(() => ((IContext)context).GetAll(typeof(object)));
		}
	}
}
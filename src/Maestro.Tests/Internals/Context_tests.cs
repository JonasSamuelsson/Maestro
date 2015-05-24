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
			Should.Throw<DependencyActivationException>(() => ((IContext)context).TryGet(typeof(object), out instance))
				.InnerException.ShouldBeOfType<ObjectDisposedException>();
			Should.Throw<DependencyActivationException>(() => ((IContext)context).GetAll(typeof(object)))
				.InnerException.ShouldBeOfType<ObjectDisposedException>();
		}
	}
}
using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests.TypeFactoryResolvers
{
	public class LazyFactoryResolver_tests
	{
		[Fact]
		public void should_resolve_unregistered_lazy_of_resolvable_type()
		{
			var o = new object();
			var container = new Container(x => x.Add<object>().Instance(o));
			var lazy = container.GetService<Lazy<object>>();
			lazy.Value.ShouldBe(o);
		}

		[Fact]
		public void should_throw_trying_to_resolve_unregistered_lazy_of_unresolvable_type()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.GetService<Lazy<object>>());
		}
	}
}
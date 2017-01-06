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
			var container = new Container();
			var lazy = container.GetService<Lazy<ResolvableClass>>();
			lazy.Value.ShouldNotBe(null);
		}

		[Fact]
		public void should_throw_trying_to_resolve_unregistered_lazy_of_unresolvable_type()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.GetService<Lazy<UnresolvableClass>>());
		}

		class ResolvableClass { }

		class UnresolvableClass
		{
			public UnresolvableClass(IDisposable disposable)
			{ }
		}
	}
}
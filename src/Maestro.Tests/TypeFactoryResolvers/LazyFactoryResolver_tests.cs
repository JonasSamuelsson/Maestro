using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests.TypeFactoryResolvers
{
	public class LazyFactoryResolver_tests
	{
		[Todo]
		public void should_get_unregistered_closed_lazy_of_resolvable_type()
		{
			var container = new Container();
			var lazy = container.Get<Lazy<ResolvableClass>>();
			lazy.Value.ShouldNotBe(null);
		}

		[Fact]
		public void should_not_get_unregistered_closed_lazy_of_unresolvable_type()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.Get<Lazy<UnresolvableClass>>());
		}

		class ResolvableClass
		{
			public ResolvableClass(object o)
			{ }
		}

		class UnresolvableClass
		{
			public UnresolvableClass(IDisposable disposable)
			{ }
		}
	}
}
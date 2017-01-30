using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class type_provider_tests
	{
		[Fact]
		public void should_use_type_providers_when_resolving_services()
		{
			new Container(x => x.TypeProviders.Add(new TypeProvider())).GetService<IFoo>().ShouldBeOfType<Foo>();
		}

		interface IFoo { }
		class Foo : IFoo { }

		class TypeProvider : ITypeProvider
		{
			public Type GetInstanceTypeOrNull(Type serviceType, IContext context)
			{
				return serviceType == typeof(IFoo) ? typeof(Foo) : null;
			}
		}
	}
}
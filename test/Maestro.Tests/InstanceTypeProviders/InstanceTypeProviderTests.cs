using Shouldly;
using System;
using Xunit;

namespace Maestro.Tests.InstanceTypeProviders
{
	public class InstanceTypeProviderTests
	{
		[Fact]
		public void ShouldGetTypeToInstantiateFromProvider()
		{
			var container = new Container(x => x.InstanceTypeProviders.Add(new FoobarTypeProvider()));

			container.TryGetService<IFoobar>(out var service).ShouldBeTrue();
			service.ShouldNotBeNull();
		}

		private class FoobarTypeProvider : IInstanceTypeProvider
		{
			public bool TryGetInstanceType(Type serviceType, Maestro.Context context, out Type instanceType)
			{
				if (serviceType == typeof(IFoobar))
				{
					instanceType = typeof(Foobar);
					return true;
				}

				instanceType = null;
				return false;
			}
		}

		private interface IFoobar { }

		private class Foobar : IFoobar { }
	}
}
using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests.TypeFactoryResolvers
{
	public class FuncFactoryResolver_tests
	{
		[Fact]
		public void should_get_unregistered_closed_func_of_resolvable_type()
		{
			var o = new object();
			var container = new Container(x => x.For<object>().Use(o));
			var func = container.Get<Func<object>>();
			func().ShouldBe(o);
		}

		[Fact]
		public void should_not_get_unregistered_closed_func_of_unresolvable_type()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.Get<Func<IInterface>>());
		}

		interface IInterface { }
	}
}
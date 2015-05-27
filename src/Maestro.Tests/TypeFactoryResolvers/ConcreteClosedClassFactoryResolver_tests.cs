using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests.TypeFactoryResolvers
{
	public class ConcreteClosedClassFactoryResolver_tests
	{
		[Fact]
		public void should_get_unregistered_concrete_closed_class()
		{
			var container = new Container();
			container.Get<object>();
		}

		[Fact]
		public void should_not_get_interface()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.Get<IDisposable>())
				.Message.ShouldBe("Can't get default instance of type System.IDisposable.");
		}

		[Fact]
		public void should_not_get_struct()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.Get<int>())
				.Message.ShouldBe("Can't get default instance of type System.Int32.");
		}
	}
}
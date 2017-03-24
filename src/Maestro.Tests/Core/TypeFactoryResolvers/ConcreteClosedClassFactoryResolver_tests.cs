using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.TypeFactoryResolvers
{
	public class ConcreteClosedClassFactoryResolver_tests
	{
		[Fact]
		public void should_get_unregistered_concrete_closed_class()
		{
			var container = new Container();
			container.GetService<EventArgs>();
		}

		[Fact]
		public void should_not_get_object()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.GetService<object>());
		}

		[Fact]
		public void should_not_get_interface()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.GetService<IDisposable>());
		}

		[Fact]
		public void should_not_get_struct()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.GetService<int>());
		}

		[Fact]
		public void should_not_get_array()
		{
			var container = new Container();
			Should.Throw<ActivationException>(() => container.GetService<int[]>());

			container.Configure(x => x.For<int>().Use.Instance(1));
			Should.Throw<ActivationException>(() => container.GetService<int[]>());
		}
	}
}
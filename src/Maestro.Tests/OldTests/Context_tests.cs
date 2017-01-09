using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class Context_tests
	{
		[Fact]
		public void Using_disposed_context_should_throw()
		{
			var container = new Container(x => x.For<Factory>().Use.Factory(ctx => new Factory(type => ctx.GetService(type))));
			var factory = container.GetService<Factory>();
			Should.Throw<ActivationException>(() => factory.Get(typeof(object))).GetBaseException().IsOfType<ObjectDisposedException>();
		}

		class Factory
		{
			private readonly Func<Type, object> _factory;

			public Factory(Func<Type, object> factory)
			{
				_factory = factory;
			}

			public object Get(Type type)
			{
				return _factory(type);
			}
		}
	}
}
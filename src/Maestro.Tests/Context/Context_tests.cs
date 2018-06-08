using System;
using Maestro.Tests.OldTests;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Context
{
	public class Context_tests
	{
		[Fact]
		public void Using_disposed_context_should_throw()
		{
			var container = new Container(x => x.Use<Factory>().Factory(ctx => new Factory(type => ctx.GetService(type))));
			var factory = container.GetService<Factory>();
			Should.Throw<ActivationException>(() => factory.Get(typeof(object))).GetBaseException().IsOfType<ObjectDisposedException>();
		}

		[Fact]
		public void context_should_expose_call_stack()
		{ }

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
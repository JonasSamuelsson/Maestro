using System;
using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class ContainerExpression_disposed_tests
	{
		[Fact]
		public void should_throw_if_container_expression_is_accessed_outside_of_closure()
		{
			ContainerConfigurator containerConfigurator = null;
			new Container().Configure(x => containerConfigurator = x);

			Should.Throw<ObjectDisposedException>(() => { var @default = containerConfigurator.Default; });

			Should.Throw<ObjectDisposedException>(() => containerConfigurator.For(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => containerConfigurator.For(typeof(object), string.Empty));

			Should.Throw<ObjectDisposedException>(() => containerConfigurator.For<object>());
			Should.Throw<ObjectDisposedException>(() => containerConfigurator.For<object>(string.Empty));

			Should.Throw<ObjectDisposedException>(() => containerConfigurator.For(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => containerConfigurator.For<object>());

			Should.Throw<ObjectDisposedException>(() => containerConfigurator.Scan(delegate { }));
		}
	}
}
using Maestro.Configuration;
using Shouldly;
using System;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class ContainerExpression_disposed_tests
	{
		[Fact]
		public void should_throw_if_container_expression_is_accessed_outside_of_closure()
		{
			ContainerBuilder containerBuilder = null;
			new Container().Configure(x => containerBuilder = x);

			Should.Throw<InvalidOperationException>(() => containerBuilder.Use(typeof(object)));
			Should.Throw<InvalidOperationException>(() => containerBuilder.Use(typeof(object), string.Empty));

			Should.Throw<InvalidOperationException>(() => containerBuilder.Use<object>());
			Should.Throw<InvalidOperationException>(() => containerBuilder.Use<object>(string.Empty));

			Should.Throw<InvalidOperationException>(() => containerBuilder.Use(typeof(object)));
			Should.Throw<InvalidOperationException>(() => containerBuilder.Use<object>());

			Should.Throw<InvalidOperationException>(() => containerBuilder.Scan(delegate { }));
		}
	}
}
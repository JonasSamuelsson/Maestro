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

			Should.Throw<InvalidOperationException>(() => containerBuilder.Add(typeof(object)));
			Should.Throw<InvalidOperationException>(() => containerBuilder.Add(typeof(object)).Named(string.Empty));

			Should.Throw<InvalidOperationException>(() => containerBuilder.Add<object>());
			Should.Throw<InvalidOperationException>(() => containerBuilder.Add<object>().Named(string.Empty));

			Should.Throw<InvalidOperationException>(() => containerBuilder.Add(typeof(object)));
			Should.Throw<InvalidOperationException>(() => containerBuilder.Add<object>());

			Should.Throw<InvalidOperationException>(() => containerBuilder.Scan(delegate { }));
		}
	}
}
using System;
using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class ContainerExpression_disposed_tests
	{
		[Fact]
		public void should_throw_if_container_expression_is_accessed_outside_of_closure()
		{
			ContainerExpression containerExpression = null;
			new Container().Configure(x => containerExpression = x);

			Should.Throw<InvalidOperationException>(() => { var @default = containerExpression.Settings; });

			Should.Throw<InvalidOperationException>(() => containerExpression.Use(typeof(object)));
			Should.Throw<InvalidOperationException>(() => containerExpression.Use(typeof(object), string.Empty));

			Should.Throw<InvalidOperationException>(() => containerExpression.Use<object>());
			Should.Throw<InvalidOperationException>(() => containerExpression.Use<object>(string.Empty));

			Should.Throw<InvalidOperationException>(() => containerExpression.Use(typeof(object)));
			Should.Throw<InvalidOperationException>(() => containerExpression.Use<object>());

			Should.Throw<InvalidOperationException>(() => containerExpression.Scan(delegate { }));
		}
	}
}
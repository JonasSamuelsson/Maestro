using Maestro.Configuration;
using Shouldly;
using System;
using Xunit;

namespace Maestro.Tests.Core
{
	public class ContainerExpression_disposed_tests
	{
		[Fact]
		public void should_throw_if_container_expression_is_accessed_outside_of_closure()
		{
			IContainerExpression containerExpression = null;
			new Container().Configure(x => containerExpression = x);

			Should.Throw<ObjectDisposedException>(() => { var @default = containerExpression.Config; });

			Should.Throw<ObjectDisposedException>(() => containerExpression.Use(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => containerExpression.Use(typeof(object), string.Empty));

			Should.Throw<ObjectDisposedException>(() => containerExpression.Use<object>());
			Should.Throw<ObjectDisposedException>(() => containerExpression.Use<object>(string.Empty));

			Should.Throw<ObjectDisposedException>(() => containerExpression.Use(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => containerExpression.Use<object>());

			Should.Throw<ObjectDisposedException>(() => containerExpression.Scan(delegate { }));
		}
	}
}
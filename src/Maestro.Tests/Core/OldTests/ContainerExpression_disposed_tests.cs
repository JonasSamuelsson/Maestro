using System;
using Maestro.Configuration;
using Shouldly;
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

			Should.Throw<ObjectDisposedException>(() => { var @default = containerExpression.Settings; });

			Should.Throw<ObjectDisposedException>(() => containerExpression.For(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => containerExpression.For(typeof(object), string.Empty));

			Should.Throw<ObjectDisposedException>(() => containerExpression.For<object>());
			Should.Throw<ObjectDisposedException>(() => containerExpression.For<object>(string.Empty));

			Should.Throw<ObjectDisposedException>(() => containerExpression.For(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => containerExpression.For<object>());

			Should.Throw<ObjectDisposedException>(() => containerExpression.Scan(delegate { }));
		}
	}
}
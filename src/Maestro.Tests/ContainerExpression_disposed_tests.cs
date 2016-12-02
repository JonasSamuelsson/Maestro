﻿using System;
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
			IContainerExpression expression = null;
			new Container().Configure(x => expression = x);

			Should.Throw<ObjectDisposedException>(() => { var @default = expression.Default; });

			Should.Throw<ObjectDisposedException>(() => { var scan = expression.Scan; });

			Should.Throw<ObjectDisposedException>(() => expression.Service(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => expression.Service(typeof(object), string.Empty));

			Should.Throw<ObjectDisposedException>(() => expression.Service<object>());
			Should.Throw<ObjectDisposedException>(() => expression.Service<object>(string.Empty));

			Should.Throw<ObjectDisposedException>(() => expression.Services(typeof(object)));
			Should.Throw<ObjectDisposedException>(() => expression.Services<object>());
		}
	}
}
﻿using System;
using Maestro.Lifetimes;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Lifetimes
{
	public class custom_lifetime
	{
		[Fact]
		public void provided_lifetime_should_be_executed()
		{
			var lifetime = new Lifetime();
			var container = new Container(x => x.For<object>().Use.Type<object>().Lifetime.Use(lifetime));

			container.GetService<object>();

			lifetime.Executed.ShouldBe(true);
		}

		private class Lifetime : ILifetime
		{
			public bool Executed { get; private set; }

			public object Execute(IContext context, Func<IContext, object> factory)
			{
				Executed = true;
				return factory(context);
			}
		}
	}
}
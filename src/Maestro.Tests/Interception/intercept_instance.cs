﻿using System.Collections.Generic;
using Maestro.Interceptors;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Interception
{
	public class intercept_instance
	{
		[Fact]
		public void interceptors_should_be_executed_in_the_same_order_they_are_configured()
		{
			var list = new List<string>();

			new Container(x => x.For<object>().Use.Self()
				.Intercept(_ => list.Add("one"))
				.Intercept(_ => list.Add("two"))
				.Intercept(_ => list.Add("three")))
			.GetService<object>();

			list.ShouldBe(new[] { "one", "two", "three" });
		}

		[Fact]
		public void interceptors_should_not_be_executed_if_instance_is_chached()
		{
			var counter = 0;

			var container = new Container(x => x.For<object>().Use.Self()
				.Intercept(_ => counter++)
				.Lifetime.Singleton());

			container.GetService<object>();
			counter.ShouldBe(1);
			container.GetService<object>();
			counter.ShouldBe(1);
		}

		[Fact]
		public void factory_instance_should_support_instance_replacement_with_wrapper_configuration()
		{
			var @object = new object();

			var container = new Container(x => x.For<object>().Use.Factory(() => @object)
				.Intercept(new FuncInterceptor<object>((instance, ctx) => new Wrapper(instance)))
				.As<Wrapper>()
				.Intercept(y => y.Text = "success"));

			var wrapper = (Wrapper)container.GetService<object>();

			wrapper.Object.ShouldBe(@object);
			wrapper.Text.ShouldBe("success");
		}

		[Fact]
		public void type_instance_should_support_instance_replacement_with_wrapper_configuration()
		{
			var container = new Container(x => x.For<object>().Use.Self()
				.Intercept(new FuncInterceptor<object>((instance, ctx) => new Wrapper(instance)))
				.As<Wrapper>()
				.Intercept(y => y.Text = "success"));

			var wrapper = (Wrapper)container.GetService<object>();

			wrapper.Object.ShouldNotBeNull();
			wrapper.Text.ShouldBe("success");
		}

		class Wrapper
		{
			public Wrapper(object @object)
			{
				Object = @object;
			}

			public object Object { get; }
			public string Text { get; set; }
		}
	}
}
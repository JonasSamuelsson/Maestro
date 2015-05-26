﻿using Shouldly;
using Xunit;

namespace Maestro.Tests.Factories
{
	public class lambda_instance_factory
	{
		[Fact]
		public void should_delegate_instantiation_to_provided_lambda()
		{
			var o = new object();

			var container = new Container(x => x.For<object>().Use(() => o));
			var instance = container.Get<object>();

			instance.ShouldBe(o);
		}

		[Fact]
		public void should_be_able_to_retrieve_dependencies()
		{
			var o = new object();
			var container = new Container(x =>
			{
				x.For<object>().Use(o);
				x.For<ClassWithDependency>().Use(ctx => new ClassWithDependency { Dependency = ctx.Get<object>() });
			});

			var instance = container.Get<ClassWithDependency>();

			instance.Dependency.ShouldBe(o);
		}

		class ClassWithDependency
		{
			public object Dependency { get; set; }
		}
	}
}
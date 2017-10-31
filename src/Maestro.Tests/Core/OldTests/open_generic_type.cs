﻿using System.Linq;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class open_generic_type
	{
		[Fact]
		public void GetService_should_support_generic_type_definitions()
		{
			var container = new Container(x => x.Use(typeof(Instance<>)).Type(typeof(Instance<>)));

			var instance = container.GetService<Instance<int>>();

			instance.ShouldBeOfType<Instance<int>>();
		}

		[Fact]
		public void should_execute_configured_interceptors()
		{
			var container = new Container(x =>
			{
				x.Use<int>().Instance(1);
				x.Use<string>().Instance("foobar");
				x.Use(typeof(Instance<>)).Type(typeof(Instance<>)).SetProperty("Value");
			});

			var instance1 = container.GetService<Instance<int>>();
			var instance2 = container.GetService<Instance<string>>();

			instance1.Value.ShouldBe(1);
			instance2.Value.ShouldBe("foobar");
		}

		[Fact]
		public void should_use_configured_lifetime()
		{
			var container = new Container(x =>
			{
				x.Use<string>().Instance("success");
				x.Use(typeof(Instance<>)).Type(typeof(Instance<>)).Lifetime.Singleton();
			});

			var instance1 = container.GetService<Instance<string>>();
			var instance2 = container.GetService<Instance<string>>();

			instance1.ShouldBe(instance2);
		}

		[Fact]
		public void child_container_config_should_be_considered_over_parent_container()
		{
			var parent = new Container(x => x.Use<Instance<string>>().Factory(() => new Instance<string> { Value = "root" }));
			var child = parent.GetChildContainer(x => x.Use(typeof(Instance<>)).Type(typeof(Instance<>)));

			var instance = child.GetService<Instance<string>>();

			instance.Value.ShouldBe(null);
		}

		[Fact]
		public void GetServices_should_support_generic_type_definitions()
		{
			var container = new Container(x => x.Add<object>().Self());

			container.GetServices<object>().Count().ShouldBe(1);
		}

		[Fact]
		public void GetServices_should_handle_container_reconfiguration()
		{
			var container = new Container(x => x.Add(typeof(Instance<>)).Self());

			container.GetServices<Instance<object>>().Count().ShouldBe(1);

			container.Configure(x => x.Add(typeof(Instance<>)).Self());

			container.GetServices<Instance<object>>().Count().ShouldBe(2);
		}

		class Instance<T>
		{
			public T Value { get; set; }
		}
	}
}
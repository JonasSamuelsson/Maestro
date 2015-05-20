﻿using System;
using System.Collections.Generic;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class Context_tests
	{
		[Fact]
		public void TryGet_configured_instance_should_pass()
		{
			var expectedInstance = new object();
			var context = new ContextBuilder()
				.AddDefault(typeof(object), new InstanceFactoryProvider(expectedInstance))
				.GetContext();

			object instance;
			context.TryGet(typeof(object), out instance).ShouldBe(true);
			instance.ShouldBeTypeOf<object>();
		}

		[Fact]
		public void Get_unconfigured_interface_instance_should_fail()
		{
			var context = new ContextBuilder().GetContext();

			object instance;
			context.TryGet(typeof(IEnumerable<>), out instance).ShouldBe(false);
			instance.ShouldBe(null);
		}

		//[Fact]
		//public void Get_all_should_get_all_configured_instances()
		//{
		//	var instance1 = "foo";
		//	var instance2 = "bar";
		//	var context = new ContextBuilder()
		//		.AddDefault(typeof (object), new InstanceFactoryProvider(instance1))
		//		.AddNamed(typeof (object), new InstanceFactoryProvider(instance1))
		//		.GetContext();
		//}

		class ContextBuilder
		{
			private readonly PluginLookup _plugins;
			private readonly Maestro.Internals.Context _context;

			public ContextBuilder(string name = null)
			{
				name = name ?? Container.DefaultName;
				_plugins = new PluginLookup();
				var kernel = new Kernel(_plugins);
				_context = new Maestro.Internals.Context(name, kernel);
			}

			public ContextBuilder AddDefault(Type type, IFactoryProvider factoryProvider)
			{
				_plugins.Add(type, Container.DefaultName, new Maestro.Internals.Plugin
				{
					FactoryProvider = factoryProvider
				});
				return this;
			}

			public Maestro.Internals.Context GetContext()
			{
				return _context;
			}
		}
	}
}
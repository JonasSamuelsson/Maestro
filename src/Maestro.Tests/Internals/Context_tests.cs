using System;
using System.Collections.Generic;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class Context_tests
	{
		[Fact]
		public void TryGet_unconfigured_abstract_type_should_fail()
		{
			var context = new ContextBuilder().GetContext();

			object instance;
			context.TryGet(typeof(IDisposable), out instance).ShouldBe(false);
			instance.ShouldBe(null);
		}

		[Fact]
		public void TryGet_configured_abstract_type_should_pass()
		{
			var expectedInstance = new object();
			var context = new ContextBuilder()
				.Add(typeof(object), new InstanceFactoryProvider(expectedInstance))
				.GetContext();

			object instance;
			context.TryGet(typeof(object), out instance).ShouldBe(true);
			instance.ShouldBeTypeOf<object>();
		}

		[Fact]
		public void TryGet_unconfigured_concrete_closed_class_instance_should_fail()
		{
			var context = new ContextBuilder().GetContext();

			object instance;
			context.TryGet(typeof(object), out instance).ShouldBe(true);
			instance.ShouldNotBe(null);
		}

		[Fact]
		public void Using_disposed_context_should_throw()
		{
			var context = new ContextBuilder().GetContext();
			context.Dispose();
			object instance;
			Should.Throw<ObjectDisposedException>(() => context.TryGet(typeof(object), out instance));
			Should.Throw<ObjectDisposedException>(() => context.GetAll(typeof(object)));
		}

		[Fact]
		public void GetAll_should_get_all_configured_instances()
		{
			var instance1 = new object();
			var instance2 = new object();
			var instance3 = new object();
			var context = new ContextBuilder()
				.Add(typeof(object), new InstanceFactoryProvider(instance1))
				.Add(typeof(object), "foo", new InstanceFactoryProvider(instance2))
				.Add(typeof(object), null, new InstanceFactoryProvider(instance3))
				.GetContext();

			context.GetAll(typeof(object)).ShouldBe(new[] { instance1, instance2, instance3 });
		}

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

			public ContextBuilder Add(Type type, IFactoryProvider factoryProvider)
			{
				return Add(type, Container.DefaultName, factoryProvider);
			}

			public ContextBuilder Add(Type type, string name, IFactoryProvider factoryProvider)
			{
				_plugins.Add(type, name, new Maestro.Internals.Plugin
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
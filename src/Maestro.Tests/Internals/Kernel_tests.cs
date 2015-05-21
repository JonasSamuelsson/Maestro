using System;
using System.Collections.Generic;
using Maestro.Internals;
using Maestro.Internals.FactoryProviders;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class Kernel_tests
	{
		[Fact]
		public void TryGet_unconfigured_abstract_type_should_fail()
		{
			var kernel = new KernelBuilder().GetKernel();
			var context = new Maestro.Internals.Context(Container.DefaultName, kernel);

			object instance;
			kernel.TryGet(typeof(IDisposable), context, out instance).ShouldBe(false);
			instance.ShouldBe(null);
		}

		[Fact]
		public void TryGet_configured_abstract_type_should_pass()
		{
			var expectedInstance = new object();
			var kernel = new KernelBuilder()
				.Add(typeof(object), new InstanceFactoryProvider(expectedInstance))
				.GetKernel();
			var context = new Maestro.Internals.Context(Container.DefaultName, kernel);

			object instance;
			kernel.TryGet(typeof(object), context, out instance).ShouldBe(true);
			instance.ShouldBeTypeOf<object>();
		}

		[Fact]
		public void TryGet_unconfigured_concrete_closed_class_instance_should_fail()
		{
			var kernel = new KernelBuilder().GetKernel();
			var context = new Maestro.Internals.Context(Container.DefaultName, kernel);

			object instance;
			kernel.TryGet(typeof(object), context, out instance).ShouldBe(true);
			instance.ShouldNotBe(null);
		}

		[Fact]
		public void GetAll_should_get_all_configured_instances()
		{
			var instance1 = new object();
			var instance2 = new object();
			var instance3 = new object();
			var kernel = new KernelBuilder()
				.Add(typeof(object), new InstanceFactoryProvider(instance1))
				.Add(typeof(object), "foo", new InstanceFactoryProvider(instance2))
				.Add(typeof(object), null, new InstanceFactoryProvider(instance3))
				.GetKernel();
			var context = new Maestro.Internals.Context(Container.DefaultName, kernel);

			kernel.GetAll(typeof(object), context).ShouldBe(new[] { instance1, instance2, instance3 });
		}

		[Fact]
		public void Resolving_a_type_with_cyclic_dependency_should_throw()
		{
			var kernel = new KernelBuilder()
				.Add(typeof(CyclicDependency), new TypeFactoryProvider(typeof(CyclicDependency)))
				.GetKernel();
			var context = new Maestro.Internals.Context(string.Empty, kernel);

			object instance;
			Should.Throw<Exception>(() => kernel.TryGet(typeof(CyclicDependency), context, out instance));
			Should.Throw<Exception>(() => kernel.GetAll(typeof(CyclicDependency), context));
		}

		class KernelBuilder
		{
			private readonly PluginLookup _plugins;
			private readonly Kernel _kernel;

			public KernelBuilder()
			{
				_plugins = new PluginLookup();
				_kernel = new Kernel(_plugins);
			}

			public KernelBuilder Add(Type type, IFactoryProvider factoryProvider)
			{
				return Add(type, Container.DefaultName, factoryProvider);
			}

			public KernelBuilder Add(Type type, string name, IFactoryProvider factoryProvider)
			{
				_plugins.Add(type, name, new Maestro.Internals.Plugin
				{
					FactoryProvider = factoryProvider
				});
				return this;
			}

			public Kernel GetKernel()
			{
				return _kernel;
			}
		}

		class CyclicDependency
		{
			public CyclicDependency(CyclicDependency dependency) { }
		}
	}
}
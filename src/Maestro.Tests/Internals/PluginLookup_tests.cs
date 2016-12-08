using System;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class PluginLookup_tests
	{
		[Fact]
		public void Should_get_plugin_matching_type_and_name()
		{
			var expectedPlugin = new TestServiceDescriptor { Type = typeof(object), Name = "foo" };
			var lookup = new PluginLookup();
			lookup.Add(expectedPlugin, true).ShouldBe(true);

			ServiceDescriptor actualDescriptor;
			lookup.TryGet(typeof(object), "foo", out actualDescriptor).ShouldBe(true);
			actualDescriptor.ShouldBe(expectedPlugin);
		}

		[Fact(Skip = "todo")]
		public void Should_get_default_plugin_if_named_plugin_cant_be_found()
		{
			var exptectedPlugin = new TestServiceDescriptor { Type = typeof(object), Name = PluginLookup.DefaultName };
			var lookup = new PluginLookup();
			lookup.Add(exptectedPlugin, true);

			ServiceDescriptor serviceDescriptor;
			lookup.TryGet(typeof(object), "not-found", out serviceDescriptor).ShouldBe(true);
			serviceDescriptor.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_all_plugins_matching_type()
		{
			var plugin1 = new TestServiceDescriptor { Type = typeof(object), Name = PluginLookup.DefaultName };
			var plugin2 = new TestServiceDescriptor { Type = typeof(object), Name = "foo" };
			var plugin3 = new TestServiceDescriptor { Type = typeof(object), Name = PluginLookup.GetRandomName() };
			var lookup = new PluginLookup();
			lookup.Add(plugin1, true);
			lookup.Add(plugin2, true);
			lookup.Add(plugin3, true);
			lookup.Add(new TestServiceDescriptor { Type = typeof(string), Name = PluginLookup.DefaultName }, true);

			lookup.GetAll(typeof(object)).ShouldBe(new[] { plugin1, plugin2, plugin3 });
		}

		[Fact]
		public void Adding_two_plugins_with_same_type_and_name_should_throw()
		{
			var lookup = new PluginLookup();

			lookup.Add(new ServiceDescriptor { Type = typeof(object), Name = PluginLookup.DefaultName }, true);
			Should.Throw<InvalidOperationException>(() => lookup.Add(new ServiceDescriptor { Type = typeof(object), Name = PluginLookup.DefaultName }, true));
		}

		[Fact]
		public void trying_to_add_two_plugins_with_same_type_and_name_should_not_throw()
		{
			var lookup = new PluginLookup();

			var plugin = new ServiceDescriptor { Type = typeof(object), Name = PluginLookup.DefaultName };

			lookup.Add(plugin, false).ShouldBe(true);
			lookup.Add(plugin, false).ShouldBe(false);
		}

		[Fact]
		public void Adding_two_anonymous_plugins_with_same_type_should_pass()
		{
			var lookup = new PluginLookup();

			lookup.Add(new ServiceDescriptor { Type = typeof(object), Name = PluginLookup.GetRandomName() }, true);
			lookup.Add(new ServiceDescriptor { Type = typeof(object), Name = PluginLookup.GetRandomName() }, true);
		}

		class TestServiceDescriptor : ServiceDescriptor
		{
			public override string ToString()
			{
				return $"plugin: {GetHashCode()}";
			}
		}
	}
}
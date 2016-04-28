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
			var expectedPlugin = new TestPlugin { Type = typeof(object), Name = "foo" };
			var lookup = new PluginLookup();
			lookup.Add(expectedPlugin);

			Plugin actualPlugin;
			lookup.TryGet(typeof(object), "foo", out actualPlugin).ShouldBe(true);
			actualPlugin.ShouldBe(expectedPlugin);
		}

		[Fact]
		public void Should_get_default_plugin_if_named_plugin_cant_be_found()
		{
			var exptectedPlugin = new TestPlugin { Type = typeof(object), Name = PluginLookup.DefaultName };
			var lookup = new PluginLookup();
			lookup.Add(exptectedPlugin);

			Plugin plugin;
			lookup.TryGet(typeof(object), "not-found", out plugin).ShouldBe(true);
			plugin.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_all_plugins_matching_type()
		{
			var plugin1 = new TestPlugin { Type = typeof(object), Name = PluginLookup.DefaultName };
			var plugin2 = new TestPlugin { Type = typeof(object), Name = "foo" };
			var plugin3 = new TestPlugin { Type = typeof(object), Name = PluginLookup.GetRandomName() };
			var lookup = new PluginLookup();
			lookup.Add(plugin1);
			lookup.Add(plugin2);
			lookup.Add(plugin3);
			lookup.Add(new TestPlugin { Type = typeof(string), Name = PluginLookup.DefaultName });

			lookup.GetAll(typeof(object)).ShouldBe(new[] { plugin1, plugin2, plugin3 });
		}

		[Fact]
		public void Adding_two_plugins_with_same_type_and_name_should_throw()
		{
			var lookup = new PluginLookup();

			lookup.Add(new Plugin { Type = typeof(object), Name = PluginLookup.DefaultName });
			Should.Throw<InvalidOperationException>(() => lookup.Add(new Plugin { Type = typeof(object), Name = PluginLookup.DefaultName }));
		}

		[Fact]
		public void Adding_two_anonymous_plugins_with_same_type_should_pass()
		{
			var lookup = new PluginLookup();

			lookup.Add(new Plugin { Type = typeof(object), Name = PluginLookup.GetRandomName() });
			lookup.Add(new Plugin { Type = typeof(object), Name = PluginLookup.GetRandomName() });
		}

		class TestPlugin : Plugin
		{
			public override string ToString()
			{
				return $"plugin: {GetHashCode()}";
			}
		}
	}
}
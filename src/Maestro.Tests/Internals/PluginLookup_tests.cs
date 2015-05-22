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
			var expectedPlugin = new TestPlugin();
			var lookup = new PluginLookup();
			lookup.Add(typeof(object), "foo", expectedPlugin);

			IPlugin actualPlugin;
			lookup.TryGet(typeof(object), "foo", out actualPlugin).ShouldBe(true);
			actualPlugin.ShouldBe(expectedPlugin);
		}

		[Fact]
		public void Should_get_default_plugin_if_named_plugin_cant_be_found()
		{
			var exptectedPlugin = new TestPlugin();
			var lookup = new PluginLookup();
			lookup.Add(typeof(object), PluginLookup.DefaultName, exptectedPlugin);

			IPlugin plugin;
			lookup.TryGet(typeof(object), "not-found", out plugin).ShouldBe(true);
			plugin.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_named_instance_from_parent_lookup()
		{
			var exptectedPlugin = new TestPlugin();
			var parentLookup = new PluginLookup();
			parentLookup.Add(typeof(object), "foo", exptectedPlugin);
			var lookup = new PluginLookup(parentLookup);
			lookup.Add(typeof(object), PluginLookup.DefaultName, new TestPlugin());

			IPlugin plugin;
			lookup.TryGet(typeof(object), "foo", out plugin).ShouldBe(true);
			plugin.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_default_instance_from_parent_lookup()
		{
			var exptectedPlugin = new TestPlugin();
			var parentLookup = new PluginLookup();
			parentLookup.Add(typeof(object), PluginLookup.DefaultName, exptectedPlugin);
			var lookup = new PluginLookup(parentLookup);
			IPlugin plugin;
			lookup.TryGet(typeof(object), "foo", out plugin).ShouldBe(true);
			plugin.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_all_plugins_matching_type()
		{
			var plugin1 = new TestPlugin();
			var plugin2 = new TestPlugin();
			var plugin3 = new TestPlugin();
			var lookup = new PluginLookup();
			lookup.Add(typeof(object), PluginLookup.DefaultName, plugin1);
			lookup.Add(typeof(object), "foo", plugin2);
			lookup.Add(typeof(object), null, plugin3);
			lookup.Add(typeof(string), null, new TestPlugin());

			lookup.GetAll(typeof(object)).ShouldBe(new[] { plugin1, plugin2, plugin3 });
		}

		[Fact]
		public void GetAll_should_not_return_plugins_from_parent_lookup_with_same_name()
		{
			var parentPlugin = new TestPlugin();
			var childPlugin1 = new TestPlugin();
			var childPlugin2 = new TestPlugin();

			var parentLookup = new PluginLookup();
			parentLookup.Add(typeof(object), "foo", new TestPlugin());
			parentLookup.Add(typeof(object), null, parentPlugin);

			var lookup = new PluginLookup(parentLookup);
			lookup.Add(typeof(object), "foo", childPlugin1);
			lookup.Add(typeof(object), null, childPlugin2);

			lookup.GetAll(typeof(object)).ShouldBe(new[] { childPlugin1, childPlugin2, parentPlugin });
		}

		[Fact]
		public void Adding_two_plugins_with_same_type_and_name_should_throw()
		{
			var lookup = new PluginLookup();

			lookup.Add(typeof(object), string.Empty, null);
			Should.Throw<InvalidOperationException>(() => lookup.Add(typeof(object), string.Empty, null));
		}

		[Fact]
		public void Adding_two_anonymous_plugins_with_same_type_should_pass()
		{
			var lookup = new PluginLookup();

			lookup.Add(typeof(object), null, null);
			lookup.Add(typeof(object), null, null);
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
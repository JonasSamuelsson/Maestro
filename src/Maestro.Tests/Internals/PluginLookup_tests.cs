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
			var expectedPlugin = new Plugin();
			var lookup = new PluginLookup();
			lookup.Add(typeof(object), "foo", expectedPlugin);

			IPlugin actualPlugin;
			lookup.TryGet(typeof(object), "foo", out actualPlugin).ShouldBe(true);
			actualPlugin.ShouldBe(expectedPlugin);
		}

		[Fact]
		public void Should_get_default_plugin_if_named_plugin_cant_be_found()
		{
			var exptectedPlugin = new Plugin();
			var lookup = new PluginLookup();
			lookup.Add(typeof(object), Container.DefaultName, exptectedPlugin);

			IPlugin plugin;
			lookup.TryGet(typeof(object), "not-found", out plugin).ShouldBe(true);
			plugin.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_named_instance_from_parent_lookup()
		{
			var exptectedPlugin = new Plugin();
			var parentLookup = new PluginLookup();
			parentLookup.Add(typeof(object), "foo", exptectedPlugin);
			var lookup = new PluginLookup(parentLookup);
			lookup.Add(typeof(object), Container.DefaultName, new Plugin());

			IPlugin plugin;
			lookup.TryGet(typeof(object), "foo", out plugin).ShouldBe(true);
			plugin.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_default_instance_from_parent_lookup()
		{
			var exptectedPlugin = new Plugin();
			var parentLookup = new PluginLookup();
			parentLookup.Add(typeof(object), Container.DefaultName, exptectedPlugin);
			var lookup = new PluginLookup(parentLookup);
			IPlugin plugin;
			lookup.TryGet(typeof(object), "foo", out plugin).ShouldBe(true);
			plugin.ShouldBe(exptectedPlugin);
		}

		[Fact]
		public void Should_get_all_plugins_matching_type()
		{
			var plugin1 = new Plugin();
			var plugin2 = new Plugin();
			var plugin3 = new Plugin();
			var lookup = new PluginLookup();
			lookup.Add(typeof(object), Container.DefaultName, plugin1);
			lookup.Add(typeof(object), "foo", plugin2);
			lookup.Add(typeof(object), null, plugin3);
			lookup.Add(typeof(string), null, new Plugin());

			lookup.GetAll(typeof(object)).ShouldBe(new[] { plugin1, plugin2, plugin3 });
		}

		[Fact]
		public void GetAll_should_not_return_plugins_from_parent_lookup_with_same_name()
		{
			var parentPlugin = new Plugin();
			var childPlugin1 = new Plugin();
			var childPlugin2 = new Plugin();

			var parentLookup = new PluginLookup();
			parentLookup.Add(typeof(object), "foo", new Plugin());
			parentLookup.Add(typeof(object), null, parentPlugin);

			var lookup = new PluginLookup(parentLookup);
			lookup.Add(typeof(object), "foo", childPlugin1);
			lookup.Add(typeof(object), null, childPlugin2);

			lookup.GetAll(typeof(object)).ShouldBe(new[] { childPlugin1, childPlugin2, parentPlugin });
		}

		class Plugin : IPlugin
		{
			public Func<object> Factory { get; set; }

			public override string ToString()
			{
				return $"plugin: {GetHashCode()}";
			}
		}
	}
}
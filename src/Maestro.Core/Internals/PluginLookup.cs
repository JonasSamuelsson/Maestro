using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	internal class PluginLookup : IPluginLookup, IDisposable
	{
		readonly List<Plugin> _list = new List<Plugin>();
		readonly PluginLookup _parent;

		public PluginLookup()
		{
		}

		public PluginLookup(PluginLookup parent)
		{
			_parent = parent;
			_parent.PluginAdded += ParentOnPluginAdded;
		}

		private void ParentOnPluginAdded()
		{
			PluginAdded();
		}

		internal static string DefaultName { get; } = string.Empty;

		public static string GetRandomName()
		{
			return Guid.NewGuid().ToString();
		}

		public event Action PluginAdded = delegate { };

		public void Add(Plugin plugin)
		{
			if (_list.Any(x => x.Type == plugin.Type && x.Name != null && x.Name == plugin.Name))
				throw new InvalidOperationException("Duplicate key");

			_list.Add(plugin);

			PluginAdded();
		}

		public bool TryGet(Type type, string name, out Plugin plugin)
		{
			plugin = GetPluginOrNull(type, name);
			return plugin != null;
		}

		private Plugin GetPluginOrNull(Type type, string name)
		{
			start:
			var lookup = this;
			do
			{
				var plugin = lookup._list.FirstOrDefault(x => x.Type == type && x.Name == name);
				if (plugin != null) return plugin;

				if (type.IsGenericType)
				{
					var genericTypeDefinition = type.GetGenericTypeDefinition();
					plugin = lookup._list.FirstOrDefault(x => x.Type == genericTypeDefinition && x.Name == name);

					if (plugin != null)
					{
						var genericArguments = type.GetGenericArguments();
						var factoryProvider = plugin.FactoryProvider.MakeGeneric(genericArguments);
						var interceptors = plugin.Interceptors.Select(x => x.MakeGeneric(genericArguments)).ToList();
						var lifetime = plugin.Lifetime.MakeGeneric(genericArguments);
						plugin = new Plugin
						{
							FactoryProvider = factoryProvider,
							Interceptors = interceptors,
							Lifetime = lifetime,
							Name = name,
							Type = type
						};
						lookup._list.Add(plugin);
						return plugin;
					}
				}

				lookup = lookup._parent;
			} while (lookup != null);

			if (name != DefaultName)
			{
				name = DefaultName;
				goto start;
			}

			return null;
		}

		public IEnumerable<Plugin> GetAll(Type type)
		{
			var lookup = this;
			var names = new HashSet<string>();
			var plugins = new List<Plugin>();
			var isGenericType = type.IsGenericType;
			var genericTypeDefinition = isGenericType ? type.GetGenericTypeDefinition() : null;

			do
			{
				foreach (var plugin in lookup._list.ToList())
				{
					if (type != plugin.Type) continue;
					if (names.Contains(plugin.Name)) continue;
					plugins.Add(plugin);
					names.Add(plugin.Name);
				}

				foreach (var plugin in lookup._list.ToList())
				{
					if (genericTypeDefinition != plugin.Type) continue;
					if (names.Contains(plugin.Name)) continue;
					var genericArguments = type.GetGenericArguments();
					var factoryProvider = plugin.FactoryProvider.MakeGeneric(genericArguments);
					var interceptors = plugin.Interceptors.Select(x => x.MakeGeneric(genericArguments)).ToList();
					var newPlugin = new Plugin
					{
						FactoryProvider = factoryProvider,
						Interceptors = interceptors,
						Lifetime = new TransientLifetime(),
						Name = plugin.Name,
						Type = type
					};
					lookup._list.Add(newPlugin);
					plugins.Add(newPlugin);
					names.Add(newPlugin.Name);
				}

				lookup = lookup._parent;
			} while (lookup != null);

			return plugins;
		}

		public void Dispose()
		{
			if (_parent == null) return;
			_parent.PluginAdded -= ParentOnPluginAdded;
		}
	}
}
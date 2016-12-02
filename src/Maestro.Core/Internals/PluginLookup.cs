using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Lifetimes;
using Maestro.Utils;

namespace Maestro.Internals
{
	internal class PluginLookup : IPluginLookup, IDisposable
	{
		readonly ThreadSafeList<Plugin> _list = new ThreadSafeList<Plugin>();

		private void ParentOnPluginAdded()
		{
			PluginAdded();
		}

		internal const string DefaultName = "";

		public static string GetRandomName()
		{
			return Guid.NewGuid().ToString();
		}

		public event Action PluginAdded = delegate { };

		public bool Add(Plugin plugin, bool throwIfDuplicate = true)
		{
			if (_list.Any(x => x.Type == plugin.Type && x.Name != null && x.Name == plugin.Name))
			{
				if (throwIfDuplicate) throw new InvalidOperationException("Duplicate key");
				return false;
			}

			_list.Add(plugin);

			PluginAdded();

			return true;
		}

		public bool TryGet(Type type, string name, out Plugin plugin)
		{
			plugin = GetPluginOrNull(type, name);
			return plugin != null;
		}

		private Plugin GetPluginOrNull(Type type, string name)
		{
			var plugin = _list.FirstOrDefault(x => x.Type == type && x.Name == name);
			if (plugin != null) return plugin;

			if (type.IsGenericType)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();
				plugin = _list.FirstOrDefault(x => x.Type == genericTypeDefinition && x.Name == name);

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
					_list.Add(plugin);
					return plugin;
				}
			}

			return null;
		}

		public IEnumerable<Plugin> GetAll(Type type)
		{
			var plugins = new List<Plugin>();
			var isGenericType = type.IsGenericType;
			var genericTypeDefinition = isGenericType ? type.GetGenericTypeDefinition() : null;

			foreach (var plugin in _list.ToList())
			{
				if (type != plugin.Type) continue;
				plugins.Add(plugin);
			}

			foreach (var plugin in _list.ToList())
			{
				if (genericTypeDefinition != plugin.Type) continue;
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
				_list.Add(newPlugin);
				plugins.Add(newPlugin);
			}

			return plugins;
		}

		public void Dispose() { }

		public static bool EqualsDefaultName(string name)
		{
			return string.IsNullOrEmpty(name);
		}
	}
}
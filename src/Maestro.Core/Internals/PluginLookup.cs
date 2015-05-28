using System;
using System.Collections.Generic;
using System.Linq;

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
		internal static string AnonymousName { get; } = null;

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
			return _list.FirstOrDefault(x => x.Type == type && x.Name == name)
					 ?? _parent?.GetPluginOrNull(type, name)
					 ?? _list.FirstOrDefault(x => x.Type == type && x.Name == DefaultName)
					 ?? _parent?.GetPluginOrNull(type, DefaultName);
		}

		public IEnumerable<Plugin> GetAll(Type type)
		{
			var names = new HashSet<string>();
			var plugins = new List<Plugin>();
			GetAll(type, names, plugins);
			return plugins;
		}

		private void GetAll(Type type, ISet<string> names, ICollection<Plugin> plugins)
		{
			foreach (var plugin in _list.Where(x => x.Type == type))
			{
				if (plugin.Name != AnonymousName) if (!names.Add(plugin.Name)) continue;
				plugins.Add(plugin);
			}

			_parent?.GetAll(type, names, plugins);
		}

		public void Dispose()
		{
			if (_parent == null) return;
			_parent.PluginAdded -= ParentOnPluginAdded;
		}
	}
}
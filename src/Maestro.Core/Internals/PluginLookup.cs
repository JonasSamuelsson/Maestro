using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	internal class PluginLookup : IPluginLookup
	{
		readonly List<Entry> _entries = new List<Entry>();
		readonly PluginLookup _parent;

		public PluginLookup()
		{
		}

		public PluginLookup(PluginLookup parent)
		{
			_parent = parent;
		}

		internal static string DefaultName { get; } = string.Empty;
		internal static string AnonymousName { get; } = null;

		public void Add(Type type, string name, IPlugin plugin)
		{
			if (_entries.Any(x => x.Type == type && x.Name != null && x.Name == name))
				throw new InvalidOperationException("Duplicate key");

			_entries.Add(new Entry
			{
				Type = type,
				Name = name,
				Plugin = plugin
			});
		}

		public bool TryGet(Type type, string name, out IPlugin plugin)
		{
			plugin = GetPluginOrNull(type, name);
			return plugin != null;
		}

		private IPlugin GetPluginOrNull(Type type, string name)
		{
			return _entries.FirstOrDefault(x => x.Type == type && x.Name == name)?.Plugin
					 ?? _parent?.GetPluginOrNull(type, name)
					 ?? _entries.FirstOrDefault(x => x.Type == type && x.Name == PluginLookup.DefaultName)?.Plugin
					 ?? _parent?.GetPluginOrNull(type, PluginLookup.DefaultName);
		}

		public IEnumerable<IPlugin> GetAll(Type type)
		{
			var names = new HashSet<string>();
			var plugins = new List<IPlugin>();
			GetAll(type, names, plugins);
			return plugins;
		}

		private void GetAll(Type type, ISet<string> names, ICollection<IPlugin> plugins)
		{
			foreach (var entry in _entries.Where(x => x.Type == type))
			{
				if (entry.Name != null) if (!names.Add(entry.Name)) continue;
				plugins.Add(entry.Plugin);
			}

			_parent?.GetAll(type, names, plugins);
		}

		class Entry
		{
			public Type Type { get; set; }
			public string Name { get; set; }
			public IPlugin Plugin { get; set; }
		}
	}
}
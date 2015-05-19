using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class PluginDictionary
	{
		private readonly PluginDictionary _parent;
		private readonly List<Entry> _entries = new List<Entry>();

		public PluginDictionary() { }

		public PluginDictionary(PluginDictionary parent)
		{
			_parent = parent;
		}

		public void Add(Type type, string name, Plugin plugin)
		{
			if (new object[] { type, name }.Any(x => x == null))
				throw new ArgumentNullException();

			if (_entries.Any(x => x.Type == type && x.Name == name))
				throw new ArgumentException();

			_entries.Add(new Entry
			{
				Type = type,
				Plugin = plugin,
				Name = name
			});
		}

		public void Add(Type type, Plugin plugin)
		{
			if (type == null)
				throw new ArgumentNullException();

			_entries.Add(new Entry
			{
				Type = type,
				Plugin = plugin,
				Name = null
			});
		}

		public bool TryGet(Type type, string name, out Plugin plugin)
		{
			var entry = _entries.FirstOrDefault(x => x.Type == type && x.Name == name)
							?? _parent?._entries.FirstOrDefault(x => x.Type == type && x.Name == name)
							?? _entries.FirstOrDefault(x => x.Type == type && x.Name == Container.DefaultName)
							?? _parent?._entries.FirstOrDefault(x => x.Type == type && x.Name == Container.DefaultName);

			plugin = entry?.Plugin;
			return plugin != null;
		}

		public IEnumerable<Plugin> GetAll(Type type)
		{
			var names = new List<string>();

			foreach (var entry in _entries)
			{
				var name = entry.Name;
				if (name != null) names.Add(name);
				yield return entry.Plugin;
			}

			foreach (var entry in _parent == null ? Enumerable.Empty<Entry>() : _parent._entries)
			{
				if (names.Contains(entry.Name)) continue;
				yield return entry.Plugin;
			}
		}

		public class Plugin
		{
			public Func<object> Factory { get; set; }
		}

		class Entry
		{
			public Type Type { get; set; }
			public string Name { get; set; }
			public Plugin Plugin { get; set; }
		}
	}
}
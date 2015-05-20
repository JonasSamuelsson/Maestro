using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	interface IBuilderLookup
	{
		void Add(string key, IBuilder builder);
		void Add(string key, IEnumerable<IBuilder> builders);
		bool TryGet(string key, out IBuilder builder);
		bool TryGet(string key, out IEnumerable<IBuilder> builders);
	}

	class BuilderLookup : IBuilderLookup
	{
		private readonly Dictionary<string, IEnumerable<IBuilder>> _dictionary = new Dictionary<string, IEnumerable<IBuilder>>();

		public void Add(string key, IBuilder builder)
		{
			Add(key, new[] { builder });
		}

		public void Add(string key, IEnumerable<IBuilder> builders)
		{
			_dictionary.Add(key, builders);
		}

		public bool TryGet(string key, out IBuilder builder)
		{
			builder = _dictionary[key].Single();
			return true;
		}

		public bool TryGet(string key, out IEnumerable<IBuilder> builders)
		{
			builders = _dictionary[key];
			return true;
		}
	}
}
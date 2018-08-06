using Maestro.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Configuration
{
	internal class Scanner : IScanner
	{
		private readonly List<IConvention> _conventions = new List<IConvention>();
		private readonly List<Func<Type, bool>> _filters = new List<Func<Type, bool>>();
		private readonly List<Type> _types = new List<Type>();

		/// <summary>
		/// Adds provided types to the list of types to process.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public IScanner Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		/// <summary>
		/// Filter types to those matching <paramref name="predicate"/>.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public IScanner Where(Func<Type, bool> predicate)
		{
			_filters.Add(predicate);
			return this;
		}

		/// <summary>
		/// Uses <paramref name="convention"/> to configure the container.
		/// </summary>
		/// <param name="convention"></param>
		public IScanner Using(IConvention convention)
		{
			_conventions.Add(convention);
			return this;
		}

		internal void Execute(ContainerBuilder containerBuilder)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.Invoke(t))).ToList();
			_conventions.ForEach(c => c.Process(types, containerBuilder));
		}
	}
}
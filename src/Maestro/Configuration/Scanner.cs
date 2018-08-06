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

		/// <inheritdoc />
		public IScanner Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}


		/// <inheritdoc />
		public IScanner Where(Func<Type, bool> predicate)
		{
			_filters.Add(predicate);
			return this;
		}

		/// <inheritdoc />
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
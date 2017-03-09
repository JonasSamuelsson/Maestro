using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	internal class ScanExpression : IScanExpression
	{
		private readonly IContainerExpression _containerExpression;
		private readonly List<Type> _types;
		private readonly List<IFilter> _filters;

		public ScanExpression(IContainerExpression containerExpression, DefaultSettings defaultSettings)
		{
			_containerExpression = containerExpression;
			_types = new List<Type>();
			_filters = new List<IFilter>(defaultSettings.GetFilters());
		}

		public IScanExpression Assemblies(IEnumerable<Assembly> assemblies)
		{
			return Types(assemblies.SelectMany(x => x.GetTypes()));
		}

		public IScanExpression Assembly(Assembly assembly)
		{
			return Assemblies(new[] { assembly });
		}

		public IScanExpression AssemblyContaining<T>()
		{
			return AssemblyContaining(typeof(T));
		}

		public IScanExpression AssemblyContaining(Type type)
		{
			return Assembly(type.Assembly);
		}

		public IScanExpression AssemblyContainingTypeOf(object o)
		{
			return AssemblyContaining(o.GetType());
		}

		public IScanExpression Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		public IScanExpression Matching(Func<Type, bool> predicate)
		{
			return Matching(new LambdaFilter(predicate));
		}

		public IScanExpression Matching(IFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		public IScanExpression With(IConvention convention)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			convention.Process(types, _containerExpression);
			return this;
		}

		public ConventionSelectorExpression For => new ConventionSelectorExpression(this);
	}
}
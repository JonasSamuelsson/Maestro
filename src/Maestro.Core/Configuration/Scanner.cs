using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	internal class Scanner : IScanner
	{
		private readonly IContainerExpression _containerExpression;
		private readonly List<Type> _types = new List<Type>();
		private readonly List<IFilter> _filters = new List<IFilter>();
		private readonly List<IConvention> _conventions = new List<IConvention>();

		public Scanner(IContainerExpression containerExpression)
		{
			_containerExpression = containerExpression;
		}

		public IScanner Assemblies(IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
			{
				Assembly(assembly);
			}

			return this;
		}

		public IScanner Assembly(Assembly assembly)
		{
			return Types(assembly.GetTypes());
		}

		public IScanner AssemblyContaining<T>()
		{
			return AssemblyContaining(typeof(T));
		}

		public IScanner AssemblyContaining(Type type)
		{
			return Assembly(type.Assembly);
		}

		public IScanner AssemblyContainingTypeOf(object o)
		{
			return AssemblyContaining(o.GetType());
		}

		public IScanner Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		public IScanner Matching(Func<Type, bool> predicate)
		{
			Matching(new LambdaFilter(predicate));
			return this;
		}

		public IScanner Matching<T>() where T : IFilter, new()
		{
			return Matching(new T());
		}

		public IScanner Matching(IFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		public IScanner With<T>() where T : IConvention, new()
		{
			return With(new T());
		}

		public IScanner With(IConvention convention)
		{
			_conventions.Add(convention);
			return this;
		}

		public void Execute()
		{
			var types = _types
				.Distinct()
				.Where(type => _filters.All(filter => filter.IsMatch(type)))
				.ToList();

			foreach (var convention in _conventions)
			{
				convention.Process(types, _containerExpression);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	internal class ConventionExpression : IConventionExpression
	{
		private readonly IContainerExpression _containerExpression;
		private readonly List<Type> _types;
		private readonly List<IConventionFilter> _filters;

		public ConventionExpression(IContainerExpression containerExpression, DefaultSettings defaultSettings)
		{
			_containerExpression = containerExpression;
			_types = new List<Type>();
			_filters = new List<IConventionFilter>(defaultSettings.GetFilters());
		}

		public IConventionExpression Assemblies(params Assembly[] assemblies)
		{
			return Types(assemblies.SelectMany(x => x.GetTypes()));
		}

		public IConventionExpression Assemblies(IEnumerable<Assembly> assemblies)
		{
			return Assemblies(assemblies.ToArray());
		}

		public IConventionExpression Assembly(Assembly assembly)
		{
			return Assemblies(new[] { assembly });
		}

		public IConventionExpression AssemblyContaining<T>()
		{
			return AssemblyContaining(typeof(T));
		}

		public IConventionExpression AssemblyContaining(Type type)
		{
			return Assembly(type.Assembly);
		}

		public IConventionExpression AssemblyContainingTypeOf(object o)
		{
			return AssemblyContaining(o.GetType());
		}

		public IConventionExpression Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		public IConventionExpression Where(Func<Type, bool> predicate)
		{
			return Matching(new LambdaFilter(predicate));
		}

		public IConventionExpression Matching(IConventionFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		public void AddConcreteSubClassesOf<T>()
		{
			AddConcreteSubClassesOf(typeof(T));
		}

		public void AddConcreteSubClassesOf(Type type)
		{
			Using(new AddConcreteSubClassesConvention(type));
		}

		public void ForConcreteClassesClosing(Type genericTypeDefinition)
		{
			Using(new ConcreteClassesClosingConvention(genericTypeDefinition));
		}

		public void ForDefaultImplementations()
		{
			Using(new DefaultImplementationConvention());
		}

		public void Using(IConvention convention)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			convention.Process(types, _containerExpression);
		}
	}
}
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
		private readonly List<IFilter> _filters;

		public ConventionExpression(IContainerExpression containerExpression, DefaultSettings defaultSettings)
		{
			_containerExpression = containerExpression;
			_types = new List<Type>();
			_filters = new List<IFilter>(defaultSettings.GetFilters());
		}

		public IConventionExpression Assemblies(IEnumerable<Assembly> assemblies)
		{
			return Types(assemblies.SelectMany(x => x.GetTypes()));
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

		public IConventionExpression Matching(Func<Type, bool> predicate)
		{
			return Matching(new LambdaFilter(predicate));
		}

		public IConventionExpression Matching(IFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		public void AddConcreteSubClassesOf<T>(Action<ITypeInstanceExpression<T>> action = null)
		{
			AddConcreteSubClassesOf(typeof(T), expression =>
														  {
															  action = action ?? delegate { };
															  var plugin = ((TypeInstanceExpression<object>)expression).ServiceDescriptor;
															  action(new TypeInstanceExpression<T>(plugin));
														  });
		}

		public void AddConcreteSubClassesOf(Type type, Action<ITypeInstanceExpression<object>> action = null)
		{
			With(new AddConcreteSubClassesConvention(type, action ?? delegate { }));
		}

		public void AddConcreteClassesClosing(Type genericTypeDefinition, Action<ITypeInstanceExpression<object>> action = null)
		{
			With(new AddConcreteClassesClosingConvention(genericTypeDefinition, action ?? delegate { }));
		}

		public void UseDefaultImplementations(Action<ITypeInstanceExpression<object>> action = null)
		{
			With(new UseDefaultImplementationsConvention(action ?? delegate { }));
		}

		public IConventionExpression With(IConvention convention)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			convention.Process(types, _containerExpression);
			return this;
		}
	}
}
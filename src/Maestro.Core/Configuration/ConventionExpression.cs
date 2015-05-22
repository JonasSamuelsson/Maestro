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

		public IConventionExpression Where(Func<Type, bool> predicate)
		{
			return Matching(new LambdaFilter(predicate));
		}

		public IConventionExpression Matching(IConventionFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		public void AddConcreteSubClassesOf<T>(Action<IInstanceBuilderExpression<T>> action = null)
		{
			throw new NotImplementedException();
			//AddConcreteSubClassesOf(typeof(T), expression =>
			//											  {
			//												  action = action ?? delegate { };
			//												  action(new InstanceBuilderExpression<T>(((InstanceBuilderExpression<object>)expression).InstanceBuilder));
			//											  });
		}

		public void AddConcreteSubClassesOf(Type type, Action<IInstanceBuilderExpression<object>> action = null)
		{
			Using(new AddConcreteSubClassesConvention(type, action ?? delegate { }));
		}

		public void AddConcreteClassesClosing(Type genericTypeDefinition, Action<IInstanceBuilderExpression<object>> action = null)
		{
			Using(new AddConcreteClassesClosingConvention(genericTypeDefinition, action ?? delegate { }));
		}

		public void UseDefaultImplementations(Action<IInstanceBuilderExpression<object>> action = null)
		{
			Using(new UseDefaultImplementationsConvention(action ?? delegate { }));
		}

		public void Using(IConvention convention)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			convention.Process(types, _containerExpression);
		}
	}
}
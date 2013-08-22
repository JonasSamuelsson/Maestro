using Maestro.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Fluent
{
	internal class ConventionExpression : IConventionExpression
	{
		private readonly IContainerConfiguration _containerConfiguration;
		private readonly List<Type> _types;
		private readonly List<IConventionFilter> _filters;

		public ConventionExpression(IContainerConfiguration containerConfiguration)
		{
			_containerConfiguration = containerConfiguration;
			_types = new List<Type>();
			_filters = new List<IConventionFilter>();
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

		public void Using(IConvention registrator)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			registrator.Process(types, _containerConfiguration);
		}

		private class LambdaFilter : IConventionFilter
		{
			private readonly Func<Type, bool> _predicate;

			public LambdaFilter(Func<Type, bool> predicate)
			{
				_predicate = predicate;
			}

			public bool IsMatch(Type type)
			{
				return _predicate(type);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Fluent
{
	internal class ConventionalRegistration : IConventionalRegistration
	{
		private readonly IContainerConfiguration _containerConfiguration;
		private readonly List<Type> _types;
		private readonly List<IConventionalRegistrationFilter> _filters;

		public ConventionalRegistration(IContainerConfiguration containerConfiguration)
		{
			_containerConfiguration = containerConfiguration;
			_types = new List<Type>();
			_filters = new List<IConventionalRegistrationFilter>();
		}

		public IConventionalRegistration Assemblies(params Assembly[] assemblies)
		{
			return Types(assemblies.SelectMany(x => x.GetTypes()));
		}

		public IConventionalRegistration Assemblies(IEnumerable<Assembly> assemblies)
		{
			return Assemblies(assemblies.ToArray());
		}

		public IConventionalRegistration Assembly(Assembly assembly)
		{
			return Assemblies(new[] { assembly });
		}

		public IConventionalRegistration AssemblyContaining<T>()
		{
			return AssemblyContaining(typeof(T));
		}

		public IConventionalRegistration AssemblyContaining(Type type)
		{
			return Assembly(type.Assembly);
		}

		public IConventionalRegistration AssemblyContainingTypeOf(object o)
		{
			return AssemblyContaining(o.GetType());
		}

		public IConventionalRegistration Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		public void AddConcreteSubClassesOf<T>()
		{
			AddConcreteSubClassesOf(typeof(T));
		}

		public void AddConcreteSubClassesOf(Type type)
		{
			Using(new ConcreteSubClassRegistrator(type));
		}

		public void AddConcreteClassesClosing(Type genericTypeDefinition)
		{
			throw new NotImplementedException();
		}

		public IConventionalRegistration Matching(Func<Type, bool> predicate)
		{
			return Matching(new LambdaFilter(predicate));
		}

		public IConventionalRegistration Matching(IConventionalRegistrationFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		public void UseDefaultImplementations()
		{
			throw new NotImplementedException();
		}

		public void Using(IConventionalRegistrator registrator)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			registrator.Process(types, _containerConfiguration);
		}

		internal class LambdaFilter : IConventionalRegistrationFilter
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
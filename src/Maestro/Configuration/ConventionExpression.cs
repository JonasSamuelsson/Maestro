using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	internal class ConventionExpression : IConventionExpression
	{
		private readonly List<Type> _types;
		private readonly List<IFilter> _filters;

		public ConventionExpression(IContainerExpression containerExpression, DefaultSettings defaultSettings)
		{
			ContainerExpression = containerExpression;
			_types = new List<Type>();
			_filters = new List<IFilter>(defaultSettings.GetFilters());
		}

		public IContainerExpression ContainerExpression { get; }

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
		
		public IConventionExpression With(IConvention convention)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			convention.Process(types, ContainerExpression);
			return this;
		}

		public IConventionSelectorExpression Use(string name = null)
		{
			ServiceRegistration serviceRegistration = name == null
				? new ServiceRegistration((serviceType, instanceType) => ContainerExpression.For(serviceType).Use.Type(instanceType))
				: (serviceType, instanceType) => ContainerExpression.For(serviceType, name).Use.Type(instanceType);
			return new ConventionSelectorExpression(this, serviceRegistration);
		}

		public IConventionSelectorExpression TryUse(string name = null)
		{
			ServiceRegistration serviceRegistration = name == null
				? new ServiceRegistration((serviceType, instanceType) => ContainerExpression.For(serviceType).TryUse.Type(instanceType))
				: (serviceType, instanceType) => ContainerExpression.For(serviceType, name).TryUse.Type(instanceType);
			return new ConventionSelectorExpression(this, serviceRegistration);
		}

		public IConventionSelectorExpression Add()
		{
			return new ConventionSelectorExpression(this, (serviceType, instanceType) => ContainerExpression.For(serviceType).Add.Type(instanceType));
		}
	}
}
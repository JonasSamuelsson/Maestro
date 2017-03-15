using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public class ScanExpression
	{
		private readonly ContainerConfigurator _containerConfigurator;
		private readonly List<Type> _types;
		private readonly List<IFilter> _filters;

		internal ScanExpression(ContainerConfigurator containerConfigurator, DefaultSettings defaultSettings)
		{
			_containerConfigurator = containerConfigurator;
			_types = new List<Type>();
			_filters = new List<IFilter>(defaultSettings.GetFilters());
		}

		/// <summary>
		/// Starting point for using out of box conventions.
		/// </summary>
		public ConventionSelector For => new ConventionSelector(this);

		/// <summary>
		/// Adds all types in <paramref name="assemblies"/> to the list of types to process.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public ScanExpression Assemblies(IEnumerable<Assembly> assemblies)
		{
			return Types(assemblies.SelectMany(x => x.GetTypes()));
		}

		/// <summary>
		/// Adds all types in <paramref name="assembly"/> to the list of types to process.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public ScanExpression Assembly(Assembly assembly)
		{
			return Assemblies(new[] { assembly });
		}

		/// <summary>
		/// Adds all types in the assembly containing type <typeparamref name="T"/> to the list of types to process.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ScanExpression AssemblyContaining<T>()
		{
			return AssemblyContaining(typeof(T));
		}

		/// <summary>
		/// Adds all types in the assembly containing type <paramref name="type"/> to the list of types to process.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ScanExpression AssemblyContaining(Type type)
		{
			return Assembly(type.Assembly);
		}

		/// <summary>
		/// Adds all types in the assembly containing type of <paramref name="o"/> to the list of types to process.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public ScanExpression AssemblyContainingTypeOf(object o)
		{
			return AssemblyContaining(o.GetType());
		}

		/// <summary>
		/// Adds provided types to the list of types to process.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public ScanExpression Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		/// <summary>
		/// Filter types to those matching <paramref name="predicate"/>.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ScanExpression Matching(Func<Type, bool> predicate)
		{
			return Matching(new LambdaFilter(predicate));
		}

		/// <summary>
		/// Filter types to those matching <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public ScanExpression Matching(IFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		/// <summary>
		/// Uses <paramref name="convention"/> to configure the container.
		/// </summary>
		/// <param name="convention"></param>
		public ScanExpression With(IConvention convention)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t)));
			convention.Process(types, _containerConfigurator);
			return this;
		}
	}
}
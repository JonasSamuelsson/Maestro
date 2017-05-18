using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	internal class Scanner : IScanner
	{
		private readonly List<IConvention> _conventions = new List<IConvention>();
		private readonly List<IFilter> _filters = new List<IFilter>();
		private readonly List<Type> _types = new List<Type>();

		internal Scanner() { }

		/// <summary>
		/// Starting point for using out of box conventions.
		/// </summary>
		public IConventionSelector For => new ConventionSelector(this);

		/// <summary>
		/// Adds all types in <paramref name="assemblies"/> to the list of types to process.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public IScanner Assemblies(IEnumerable<Assembly> assemblies)
		{
			return Types(assemblies.SelectMany(x => x.GetTypes()));
		}

		/// <summary>
		/// Adds all types in <paramref name="assembly"/> to the list of types to process.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public IScanner Assembly(Assembly assembly)
		{
			return Assemblies(new[] { assembly });
		}

		/// <summary>
		/// Adds all types in the assembly containing type <typeparamref name="T"/> to the list of types to process.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IScanner AssemblyContaining<T>()
		{
			return AssemblyContaining(typeof(T));
		}

		/// <summary>
		/// Adds all types in the assembly containing type <paramref name="type"/> to the list of types to process.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IScanner AssemblyContaining(Type type)
		{
			return Assembly(type.GetAssembly());
		}

		/// <summary>
		/// Adds all types in the assembly containing type of <paramref name="o"/> to the list of types to process.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public IScanner AssemblyContainingTypeOf(object o)
		{
			return AssemblyContaining(o.GetType());
		}

		/// <summary>
		/// Adds provided types to the list of types to process.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public IScanner Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		/// <summary>
		/// Filter types to those matching <paramref name="predicate"/>.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public IScanner Where(Func<Type, bool> predicate)
		{
			return Matching(new LambdaFilter(predicate));
		}

		/// <summary>
		/// Filter types to those matching <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public IScanner Matching(IFilter filter)
		{
			_filters.Add(filter);
			return this;
		}

		/// <summary>
		/// Uses <paramref name="convention"/> to configure the container.
		/// </summary>
		/// <param name="convention"></param>
		public IScanner Using(IConvention convention)
		{
			_conventions.Add(convention);
			return this;
		}

		public IScanner Using<TConvention>() where TConvention : IConvention, new()
		{
			return Using(new TConvention());
		}

		internal void Execute(ContainerExpression containerExpression)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.IsMatch(t))).ToList();
			_conventions.ForEach(c => c.Process(types, containerExpression));
		}
	}
}
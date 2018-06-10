using Maestro.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Configuration
{
	public class Scanner
	{
		private readonly List<IConvention> _conventions = new List<IConvention>();
		private readonly List<Func<Type, bool>> _filters = new List<Func<Type, bool>>();
		private readonly List<Type> _types = new List<Type>();

		internal Scanner() { }

		/// <summary>
		/// Adds all types in <paramref name="assemblies"/> to the list of types to process.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public Scanner Assemblies(IEnumerable<Assembly> assemblies)
		{
			return Types(assemblies.SelectMany(x => x.GetTypes()));
		}

		/// <summary>
		/// Adds all types in <paramref name="assembly"/> to the list of types to process.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public Scanner Assembly(Assembly assembly)
		{
			return Assemblies(new[] { assembly });
		}

		/// <summary>
		/// Adds all types in the assembly containing type <typeparamref name="T"/> to the list of types to process.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public Scanner AssemblyContaining<T>()
		{
			return AssemblyContaining(typeof(T));
		}

		/// <summary>
		/// Adds all types in the assembly containing type <paramref name="type"/> to the list of types to process.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Scanner AssemblyContaining(Type type)
		{
			return Assembly(type.Assembly);
		}

		/// <summary>
		/// Adds all types in the assembly containing type of <paramref name="o"/> to the list of types to process.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public Scanner AssemblyContainingTypeOf(object o)
		{
			return AssemblyContaining(o.GetType());
		}

		/// <summary>
		/// Adds provided types to the list of types to process.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public Scanner Types(IEnumerable<Type> types)
		{
			_types.AddRange(types);
			return this;
		}

		/// <summary>
		/// Filter types to those matching <paramref name="predicate"/>.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public Scanner Where(Func<Type, bool> predicate)
		{
			_filters.Add(predicate);
			return this;
		}

		/// <summary>
		/// Uses <paramref name="convention"/> to configure the container.
		/// </summary>
		/// <param name="convention"></param>
		public Scanner Using(IConvention convention)
		{
			_conventions.Add(convention);
			return this;
		}

		/// <summary>
		/// Instantiates and uses <typeparamref name="TConvention"/> to configure the container.
		/// </summary>
		public Scanner Using<TConvention>() where TConvention : IConvention, new()
		{
			return Using(new TConvention());
		}

		public Scanner RegisterConcreteClassesOf<T>(Action<IConventionalServiceExpression<T>> action = null)
		{
			return Using(new ConcreteClassesOfConvention<T>(typeof(T), action ?? (x => x.Add())));
		}

		public Scanner RegisterConcreteClassesOf(Type type, Action<IConventionalServiceExpression<object>> action = null)
		{
			return Using(new ConcreteClassesOfConvention<object>(type, action ?? (x => x.Add())));
		}

		public Scanner RegisterConcreteClassesClosing(Type genericTypeDefinition, Action<IConventionalServiceExpression<object>> action = null)
		{
			return Using(new ConcreteClassesClosingConvention(genericTypeDefinition, action ?? (x => x.Add())));
		}

		public Scanner RegisterDefaultImplementations(Action<IConventionalServiceExpression<object>> action = null)
		{
			return Using(new DefaultImplementationsConvention(action ?? (x => x.Use())));
		}

		internal void Execute(ContainerExpression containerExpression)
		{
			var types = _types.Distinct().Where(t => _filters.All(f => f.Invoke(t))).ToList();
			_conventions.ForEach(c => c.Process(types, containerExpression));
		}
	}
}
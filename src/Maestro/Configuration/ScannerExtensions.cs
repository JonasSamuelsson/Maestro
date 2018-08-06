using Maestro.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Configuration
{
	public static class ScannerExtensions
	{
		/// <summary>
		/// Adds all types in <paramref name="assemblies"/> to the list of types to process.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public static IScanner Assemblies(this IScanner scanner, IEnumerable<Assembly> assemblies)
		{
			return scanner.Types(assemblies.SelectMany(x => x.GetTypes()));
		}

		/// <summary>
		/// Adds all types in <paramref name="assembly"/> to the list of types to process.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IScanner Assembly(this IScanner scanner, Assembly assembly)
		{
			return scanner.Assemblies(new[] { assembly });
		}

		/// <summary>
		/// Adds all types in the assembly containing type <typeparamref name="T"/> to the list of types to process.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IScanner AssemblyContaining<T>(this IScanner scanner)
		{
			return scanner.AssemblyContaining(typeof(T));
		}

		/// <summary>
		/// Adds all types in the assembly containing type <paramref name="type"/> to the list of types to process.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IScanner AssemblyContaining(this IScanner scanner, Type type)
		{
			return scanner.Assembly(type.Assembly);
		}

		/// <summary>
		/// Adds all types in the assembly containing type of <paramref name="o"/> to the list of types to process.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static IScanner AssemblyContainingTypeOf(this IScanner scanner, object o)
		{
			return scanner.AssemblyContaining(o.GetType());
		}

		/// <summary>
		/// Instantiates and uses <typeparamref name="TConvention"/> to configure the container.
		/// </summary>
		public static IScanner Using<TConvention>(this IScanner scanner) where TConvention : IConvention, new()
		{
			return scanner.Using(new TConvention());
		}

		public static IScanner RegisterConcreteClassesOf<T>(this IScanner scanner, Action<IConventionalServiceRegistrator<T>> action = null)
		{
			return scanner.Using(new ConcreteClassesOfConvention<T>(typeof(T), action ?? (x => x.Add())));
		}

		public static IScanner RegisterConcreteClassesOf(this IScanner scanner, Type type, Action<IConventionalServiceRegistrator<object>> action = null)
		{
			return scanner.Using(new ConcreteClassesOfConvention<object>(type, action ?? (x => x.Add())));
		}

		public static IScanner RegisterConcreteClassesClosing(this IScanner scanner, Type genericTypeDefinition, Action<IConventionalServiceRegistrator<object>> action = null)
		{
			return scanner.Using(new ConcreteClassesClosingConvention(genericTypeDefinition, action ?? (x => x.Add())));
		}

		public static IScanner RegisterDefaultImplementations(this IScanner scanner, Action<IConventionalServiceRegistrator<object>> action = null)
		{
			return scanner.Using(new DefaultImplementationsConvention(action ?? (x => x.Add())));
		}
	}
}
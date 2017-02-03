using System;
using System.Collections.Generic;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public interface IConventionExpression
	{
		/// <summary>
		/// Adds all types in <paramref name="assemblies"/> to the list of types to process.
		/// </summary>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		IConventionExpression Assemblies(IEnumerable<Assembly> assemblies);

		/// <summary>
		/// Adds all types in <paramref name="assembly"/> to the list of types to process.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		IConventionExpression Assembly(Assembly assembly);

		/// <summary>
		/// Adds all types in the assembly containing type <typeparamref name="T"/> to the list of types to process.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IConventionExpression AssemblyContaining<T>();

		/// <summary>
		/// Adds all types in the assembly containing type <paramref name="type"/> to the list of types to process.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IConventionExpression AssemblyContaining(Type type);

		/// <summary>
		/// Adds all types in the assembly containing type of <paramref name="o"/> to the list of types to process.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		IConventionExpression AssemblyContainingTypeOf(object o);

		/// <summary>
		/// Adds <paramref name="types"/> to the list of types to process.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		IConventionExpression Types(IEnumerable<Type> types);

		/// <summary>
		/// Filter types to those matching <paramref name="predicate"/>.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IConventionExpression Matching(Func<Type, bool> predicate);

		/// <summary>
		/// Filter types to those matching <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		IConventionExpression Matching(IFilter filter);

		/// <summary>
		/// Adds concrete sub classes of <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		void AddConcreteSubClassesOf<T>(Action<ITypeInstanceExpression<T>> action = null);

		/// <summary>
		/// Adds concrete sub classes of <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="action"></param>
		void AddConcreteSubClassesOf(Type type, Action<ITypeInstanceExpression<object>> action = null);

		/// <summary>
		/// Adds implementations of <paramref name="genericTypeDefinition"/>.
		/// </summary>
		/// <param name="genericTypeDefinition"></param>
		/// <param name="action"></param>
		void AddConcreteClassesClosing(Type genericTypeDefinition, Action<ITypeInstanceExpression<object>> action = null);

		/// <summary>
		/// Adds default implementations.
		/// </summary>
		/// <param name="action"></param>
		void UseDefaultImplementations(Action<ITypeInstanceExpression<object>> action = null);

		/// <summary>
		/// Uses <paramref name="convention"/> to configure the container.
		/// </summary>
		/// <param name="convention"></param>
		IConventionExpression With(IConvention convention);
	}
}
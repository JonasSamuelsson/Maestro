using System;
using System.Collections.Generic;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
   public interface IScanner
   {
      /// <summary>
      /// Starting point for using out of box conventions.
      /// </summary>
      IConventionSelector For { get; }

      /// <summary>
      /// Adds all types in <paramref name="assemblies"/> to the list of types to process.
      /// </summary>
      /// <param name="assemblies"></param>
      /// <returns></returns>
      IScanner Assemblies(IEnumerable<Assembly> assemblies);

      /// <summary>
      /// Adds all types in <paramref name="assembly"/> to the list of types to process.
      /// </summary>
      /// <param name="assembly"></param>
      /// <returns></returns>
      IScanner Assembly(Assembly assembly);

      /// <summary>
      /// Adds all types in the assembly containing type <typeparamref name="T"/> to the list of types to process.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <returns></returns>
      IScanner AssemblyContaining<T>();

      /// <summary>
      /// Adds all types in the assembly containing type <paramref name="type"/> to the list of types to process.
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      IScanner AssemblyContaining(Type type);

      /// <summary>
      /// Adds all types in the assembly containing type of <paramref name="o"/> to the list of types to process.
      /// </summary>
      /// <param name="o"></param>
      /// <returns></returns>
      IScanner AssemblyContainingTypeOf(object o);

      /// <summary>
      /// Adds provided types to the list of types to process.
      /// </summary>
      /// <param name="types"></param>
      /// <returns></returns>
      IScanner Types(IEnumerable<Type> types);

      /// <summary>
      /// Filter types to those matching <paramref name="predicate"/>.
      /// </summary>
      /// <param name="predicate"></param>
      /// <returns></returns>
      IScanner Matching(Func<Type, bool> predicate);

      /// <summary>
      /// Filter types to those matching <paramref name="filter"/>.
      /// </summary>
      /// <param name="filter"></param>
      /// <returns></returns>
      IScanner Matching(IFilter filter);

      /// <summary>
      /// Uses <paramref name="convention"/> to configure the container.
      /// </summary>
      /// <param name="convention"></param>
      IScanner With(IConvention convention);
   }
}
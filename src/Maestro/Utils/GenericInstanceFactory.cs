using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Utils
{
	internal static class GenericInstanceFactory
	{
		private static readonly ConcurrentDictionary<Type, Func<object, Type[], object>> Cache = new ConcurrentDictionary<Type, Func<object, Type[], object>>();

		public static T Create<T>(object source, Type[] genericArguments)
		{
			var type = source.GetType();
			return (T)Cache.GetOrAdd(type, GetFactory).Invoke(source, genericArguments);
		}

		private static Func<object, Type[], object> GetFactory(Type type)
		{
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

			var method = type.GetMethod("MakeGeneric", bindingFlags);
			if (method != null)
			{
				var parameters = method.GetParameters();
				if (parameters.Length == 1)
				{
					var parameterType = parameters[0].ParameterType;
					if (parameterType == typeof(Type[]))
					{
						return (source, types) => method.Invoke(source, new object[] { types });
					}
				}
			}

			if (type.GetConstructors(bindingFlags).Any(x => x.GetParameters().Length == 0))
			{
				return (source, types) => Activator.CreateInstance(type);
			}

			throw new InvalidOperationException();
		}
	}
}

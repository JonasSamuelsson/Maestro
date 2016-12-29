using System;
using System.Collections.Generic;

namespace Maestro
{
	public static class ContainerExtensions
	{
		public static object Get(this IContainer container, Type type, string name = null)
		{
			return container.GetService(type, name);
		}

		public static T Get<T>(this IContainer container, string name = null)
		{
			return container.GetService<T>(name);
		}

		public static IEnumerable<object> GetAll(this IContainer container, Type type)
		{
			return container.GetServices(type);
		}

		public static IEnumerable<T> GetAll<T>(this IContainer container)
		{
			return container.GetServices<T>();
		}

		public static bool TryGet(this IContainer container, Type type, out object instance)
		{
			return container.TryGetService(type, out instance);
		}

		public static bool TryGet(this IContainer container, Type type, string name, out object instance)
		{
			return container.TryGetService(type, name, out instance);
		}

		public static bool TryGet<T>(this IContainer container, out T instance)
		{
			return container.TryGetService(out instance);
		}

		public static bool TryGet<T>(this IContainer container, string name, out T instance)
		{
			return container.TryGetService(name, out instance);
		}
	}
}
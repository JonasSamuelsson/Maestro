using System;
using System.Collections.Generic;

namespace Maestro
{
	public static class ContextExtensions
	{
		public static bool CanGet(this IContext context, Type type)
		{
			return context.CanGetService(type);
		}

		public static bool CanGet<T>(this IContext context)
		{
			return context.CanGetService<T>();
		}

		public static object Get(this IContext context, Type type, string name = null)
		{
			return context.Get(type, name);
		}

		public static T Get<T>(this IContext context, string name = null)
		{
			return context.Get<T>(name);
		}

		public static IEnumerable<object> GetAll(this IContext context, Type type)
		{
			return context.GetServices(type);
		}

		public static IEnumerable<T> GetAll<T>(this IContext context)
		{
			return context.GetServices<T>();
		}

		public static bool TryGet(this IContext context, Type type, out object instance)
		{
			return context.TryGetService(type, out instance);
		}

		public static bool TryGet(this IContext context, Type type, string name, out object instance)
		{
			return context.TryGet(type, name, out instance);
		}

		public static bool TryGet<T>(this IContext context, out T instance)
		{
			return context.TryGetService(out instance);
		}

		public static bool TryGet<T>(this IContext context, string name, out T instance)
		{
			return context.TryGet(name, out instance);
		}
	}
}
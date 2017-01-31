using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro.Interceptors
{
	static class PropertySetter
	{
		static readonly Dictionary<PropertyInfo, Action> Cache = new Dictionary<PropertyInfo, Action>();

		public static Action Create(PropertyInfo property)
		{
			Action action;
			if (Cache.TryGetValue(property, out action))
				return action;

			var target = Expression.Parameter(typeof(object), "target");
			var value = Expression.Parameter(typeof(object), "value");
			var typedTarget = Expression.Convert(target, property.ReflectedType);
			var typedValue = Expression.Convert(value, property.PropertyType);
			var assignment = Expression.Assign(Expression.Property(typedTarget, property), typedValue);
			return Cache[property] = Expression.Lambda<Action>(assignment, target, value).Compile();
		}

		internal delegate void Action(object target, object value);
	}
}
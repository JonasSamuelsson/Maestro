using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro.Interceptors
{
	static class SetPropertyActionFactory
	{
		static readonly Dictionary<PropertyInfo, SetPropertyAction> Cache = new Dictionary<PropertyInfo, SetPropertyAction>();

		public static SetPropertyAction Get(PropertyInfo property)
		{
			SetPropertyAction action;
			if (Cache.TryGetValue(property, out action))
				return action;

			var target = Expression.Parameter(typeof(object), "target");
			var value = Expression.Parameter(typeof(object), "value");
			var typedTarget = Expression.Convert(target, property.ReflectedType);
			var typedValue = Expression.Convert(value, property.PropertyType);
			var assignment = Expression.Assign(Expression.Property(typedTarget, property), typedValue);
			return Cache[property] = Expression.Lambda<SetPropertyAction>(assignment, target, value).Compile();
		}
	}
}
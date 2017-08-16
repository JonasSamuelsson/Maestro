using Maestro.Utils;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro.Interceptors
{
	static class PropertySetter
	{
		static readonly ThreadSafeDictionary<PropertyInfo, Action> Cache = new ThreadSafeDictionary<PropertyInfo, Action>();

		public static Action Create(PropertyInfo property)
		{
			Action action;
			if (Cache.TryGet(property, out action))
				return action;

			var target = Expression.Parameter(typeof(object), "target");
			var value = Expression.Parameter(typeof(object), "value");
			var typedTarget = Expression.Convert(target, property.DeclaringType);
			var typedValue = Expression.Convert(value, property.PropertyType);
			var assignment = Expression.Assign(Expression.Property(typedTarget, property), typedValue);
			action = Expression.Lambda<Action>(assignment, target, value).Compile();
			Cache.Set(property, action);
			return action;
		}

		internal delegate void Action(object target, object value);
	}
}
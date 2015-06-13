using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Maestro.Internals;
using Maestro.Utils;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : IInterceptor
	{
		private readonly string _propertyName;
		private Func<IContext, object> _factory;

		public SetPropertyInterceptor(string propertyName, Func<IContext, object> factory = null)
		{
			_propertyName = propertyName;
			_factory = factory;
		}

		public object Execute(object instance, IContext context)
		{
			var property = instance.GetType().GetProperty(_propertyName);
			var action = PropertyAssignment.Get(property, _factory);
			action.Invoke(instance, (Context)context);
			return instance;
		}

		public override string ToString()
		{
			return string.Format("set property {0}", _propertyName);
		}
	}

	class PropertyAssignment
	{
		static readonly Dictionary<PropertyInfo, Action<object, Context>> Cache = new Dictionary<PropertyInfo, Action<object, Context>>();

		public static Action<object, Context> Get(PropertyInfo property, Func<IContext, object> dependencyProvider = null)
		{
			Action<object, Context> action;
			if (Cache.TryGetValue(property, out action))
				return action;

			var target = Expression.Parameter(typeof(object), "target");
			var context = Expression.Parameter(typeof(Context), "ctx");
			var func = dependencyProvider == null ? GetFunc(property.PropertyType) : GetFunc(dependencyProvider);
			var value = Expression.Convert(Expression.Invoke(func, context), property.PropertyType);
			var assignment = Expression.Assign(Expression.Property(Expression.Convert(target, property.ReflectedType), property), value);
			return Cache[property] = Expression.Lambda<Action<object, Context>>(assignment, target, context).Compile();
		}

		static Expression<Func<Context, object>> GetFunc(Type type)
		{
			Expression<Func<Context, object>> func = ctx => ctx.Kernel.GetDependency(type, ctx);
			return func;
		}

		static Expression<Func<Context, object>> GetFunc(Func<IContext, object> dependencyProvider)
		{
			Expression<Func<Context, object>> func = ctx => dependencyProvider(ctx);
			return func;
		}
	}
}
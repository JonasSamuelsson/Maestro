using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro.Utils
{
	internal static class Reflector
	{
		public static bool AlwaysUseReflection = false;

		private static bool TryGetExpressionCompiler<T>(out MethodInfo compiler)
		{
			if (AlwaysUseReflection)
			{
				compiler = null;
				return false;
			}

			var methods = typeof(Expression<T>).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			compiler = methods.SingleOrDefault(x => x.Name == "Compile" && x.GetParameters().Length == 0);
			return compiler != null;
		}

		private static Func<IContext, object> GetValueProviderOrNull(Type type, IContext context, bool resolveValueTypeArrays)
		{
			if (context.CanGetService(type))
				return GetValueProvider(type);

			if (type.IsArray && (resolveValueTypeArrays || !type.GetElementType().IsValueType))
				return GetArrayProvider(type.GetElementType());

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) && !type.GetGenericArguments().Single().IsValueType)
				return GetEnumerableProvider(type.GetGenericArguments().Single());

			return null;
		}

		private static Func<IContext, object> GetArrayProvider(Type type)
		{
			var getAllMethod = typeof(IContext).GetMethod("GetAll", new Type[0]).MakeGenericMethod(type);
			var toArrayMethod = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(type);

			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Func<IContext, object>>(out compiler))
				return ctx => toArrayMethod.Invoke(null, new[] { getAllMethod.Invoke(ctx, null) });

			var context = Expression.Parameter(typeof(IContext), "context");
			var enumerable = Expression.Call(context, getAllMethod);
			var array = Expression.Call(toArrayMethod, new Expression[] { enumerable });
			var lambda = Expression.Lambda<Func<IContext, object>>(array, new[] { context });
			return (Func<IContext, object>)compiler.Invoke(lambda, null);
		}

		private static Func<IContext, object> GetEnumerableProvider(Type type)
		{
			var getAllMethod = typeof(IContext).GetMethod("GetAll", new Type[0]).MakeGenericMethod(type);

			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Func<IContext, object>>(out compiler))
				return ctx => getAllMethod.Invoke(ctx, null);

			var context = Expression.Parameter(typeof(IContext), "context");
			var enumerable = Expression.Call(context, getAllMethod);
			var lambda = Expression.Lambda<Func<IContext, object>>(enumerable, new[] { context });
			return (Func<IContext, object>)compiler.Invoke(lambda, null);
		}

		private static Func<IContext, object> GetValueProvider(Type type)
		{
			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Func<IContext, object>>(out compiler))
				return ctx => ctx.GetService(type);

			var context = Expression.Parameter(typeof(IContext), "context");
			var getMethod = typeof(IContext).GetMethod("Get", new[] { typeof(Type) });
			var value = Expression.Call(context, getMethod, new Expression[] { Expression.Constant(type) });
			var lambda = Expression.Lambda<Func<IContext, object>>(value, new[] { context });
			return (Func<IContext, object>)compiler.Invoke(lambda, null);
		}

		public static Action<object, object> GetPropertySetter(Type instanceType, string propertyName)
		{
			var property = instanceType.GetProperty(propertyName);
			var setMethod = property.GetSetMethod();

			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Action<object, object>>(out compiler))
				return (inst, val) => setMethod.Invoke(inst, new[] { val });

			var instance = Expression.Parameter(typeof(object), "instance");
			var value = Expression.Parameter(typeof(object), "value");
			var typedInstance = Expression.Convert(instance, instanceType);
			var typedValue = Expression.Convert(value, property.PropertyType);
			var call = Expression.Call(typedInstance, setMethod, new Expression[] { typedValue });
			var lambda = Expression.Lambda<Action<object, object>>(call, new[] { instance, value });
			return (Action<object, object>)compiler.Invoke(lambda, null);
		}

		public static Func<IContext, object> GetPropertyValueProvider(Type type, IContext context)
		{
			return GetValueProviderOrNull(type, context, true) ?? (ctx => ctx.GetService(type));
		}

		public static Func<IContext, object> GetInstantiatorOrNull(Type type, IContext context)
		{
			foreach (var constructor in type.GetConstructors().OrderByDescending(x => x.GetParameters().Length))
			{
				var ctor = constructor; // prevents access to modified closure
				var parameterTypes = ctor.GetParameters().Select(x => x.ParameterType).ToList();
				var providers = parameterTypes.Select(x => new { @delegate = GetValueProviderOrNull(x, context, false), type = x }).ToList();
				if (providers.Any(x => x.@delegate == null))
					continue;

				MethodInfo compiler;
				if (!TryGetExpressionCompiler<Func<IContext, object>>(out compiler))
					return ctx => ctor.Invoke(providers.Select(x => x.@delegate.Invoke(ctx)).ToArray());

				var ctxExpression = Expression.Parameter(typeof(IContext), "context");
				var arguments = providers.Select(x =>
															{
																Expression<Func<IContext, object>> @delegate = ctx => x.@delegate.Invoke(ctx);
																var call = Expression.Invoke(@delegate, new Expression[] { ctxExpression });
																return Expression.Convert(call, x.type);
															});
				var @new = Expression.New(ctor, arguments.Cast<Expression>().ToArray());
				var lambda = Expression.Lambda<Func<IContext, object>>(@new, new[] { ctxExpression });
				return (Func<IContext, object>)compiler.Invoke(lambda, null);
			}

			return null;
		}

		public static string GetName<T, TValue>(this Expression<Func<T, TValue>> property)
		{
			return ((MemberExpression)property.Body).Member.Name;
		}

		public static bool IsGenericEnumerable(Type type)
		{
			Type elementType;
			return IsGenericEnumerable(type, out elementType);
		}

		public static bool IsGenericEnumerable(Type type, out Type elementType)
		{
			elementType = null;
			if (!type.IsGenericType) return false;
			var genericTypeDefinition = type.GetGenericTypeDefinition();
			if (genericTypeDefinition != typeof(IEnumerable<>)) return false;
			elementType = type.GetGenericArguments().Single();
			return true;
		}

		public static bool IsNonPrimitiveGenericEnumerable(Type type)
		{
			Type elementType;
			return IsNonPrimitiveGenericEnumerable(type, out elementType);
		}

		public static bool IsNonPrimitiveGenericEnumerable(Type type, out Type elementType)
		{
			if (!IsGenericEnumerable(type, out elementType))
			{
				return false;
			}

			if (IsPrimitive(elementType))
			{
				elementType = null;
				return false;
			}

			return true;
		}

		public static bool IsPrimitiveGenericEnumerable(Type type)
		{
			Type elementType;
			return IsPrimitiveGenericEnumerable(type, out elementType);
		}

		public static bool IsPrimitiveGenericEnumerable(Type type, out Type elementType)
		{
			if (!IsGenericEnumerable(type, out elementType))
			{
				return false;
			}

			if (!IsPrimitive(elementType))
			{
				elementType = null;
				return false;
			}

			return true;
		}

		public static bool IsPrimitive(Type type)
		{
			return type.IsValueType || type == typeof(string) || type == typeof(object);
		}

		public static bool IsGeneric(Type type, out Type genericTypeDefinition, out Type[] genericArguments)
		{
			genericTypeDefinition = null;
			genericArguments = null;

			if (!type.IsGenericType)
			{
				return false;
			}

			if (type.IsGenericTypeDefinition)
			{
				return false;
			}

			genericTypeDefinition = type.GetGenericTypeDefinition();
			genericArguments = type.GetGenericArguments();
			return true;
		}
	}
}
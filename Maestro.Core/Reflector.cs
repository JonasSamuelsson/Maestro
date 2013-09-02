using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro
{
	internal static class Reflector
	{
		public static bool AlwaysUseReflection;

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

		public static Func<IContext, object> GetConstructorValueProvider(Type type, IContext context)
		{
			return GetValueProvider(false, type, context);
		}

		private static Func<IContext, object> GetValueProvider(bool resolveValueTypeArrays, Type type, IContext context)
		{
			if (context.CanGet(type))
				return GetValueProvider(type);

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				return GetEnumerableProvider(type.GetGenericArguments().Single());

			if (type.IsArray && (resolveValueTypeArrays || !type.GetElementType().IsValueType))
				return GetArrayProvider(type.GetElementType());

			return GetValueProvider(type);
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
			var array = Expression.Call(toArrayMethod, new[] { enumerable });
			var lambda = Expression.Lambda<Func<IContext, object>>(array, new[] { context });
			return (Func<IContext, object>)compiler.Invoke(lambda, null);
		}

		private static Func<IContext, object> GetEnumerableProvider(Type type)
		{
			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Func<IContext, object>>(out compiler))
				return ctx => ctx.GetAll(type);

			var context = Expression.Parameter(typeof(IContext), "context");
			var getAllMethod = typeof(IContext).GetMethod("GetAll", new Type[0]).MakeGenericMethod(type);
			var enumerable = Expression.Call(context, getAllMethod);
			var lambda = Expression.Lambda<Func<IContext, object>>(enumerable, new[] { context });
			return (Func<IContext, object>)compiler.Invoke(lambda, null);
		}

		private static Func<IContext, object> GetValueProvider(Type type)
		{
			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Func<IContext, object>>(out compiler))
				return ctx => ctx.Get(type);

			var context = Expression.Parameter(typeof(IContext), "context");
			var getMethod = typeof(IContext).GetMethod("Get", new[] { typeof(Type) });
			var value = Expression.Call(context, getMethod, Expression.Constant(type));
			var lambda = Expression.Lambda<Func<IContext, object>>(value, new[] { context });
			return (Func<IContext, object>)compiler.Invoke(lambda, null);
		}

		public static Func<object[], object> GetConstructorCall(ConstructorInfo ctor)
		{
			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Func<object[], object>>(out compiler))
				return new ReflectionInstantiator(ctor).Instantiate;

			var parameterTypes = ctor.GetParameters().Select(x => x.ParameterType).ToList();
			var args = Expression.Parameter(typeof(object[]), "args");
			var typedArgs = new Expression[parameterTypes.Count];
			for (var i = 0; i < parameterTypes.Count; i++)
			{
				var parameterType = parameterTypes[i];
				var arg = Expression.ArrayIndex(args, Expression.Constant(i));
				typedArgs[i] = Expression.Convert(arg, parameterType);
			}
			var @new = Expression.New(ctor, typedArgs);
			var lambda = Expression.Lambda<Func<object[], object>>(@new, args);
			return (Func<object[], object>)compiler.Invoke(lambda, null);
		}

		public static Action<object, object> GetPropertySetter(Type instanceType, string propertyName)
		{
			MethodInfo compiler;
			if (!TryGetExpressionCompiler<Action<object, object>>(out compiler))
				return new ReflectionPropertySetter(propertyName).SetPropertery;

			var property = instanceType.GetProperty(propertyName);
			var setMethod = property.GetSetMethod();
			var instance = Expression.Parameter(typeof(object), "instance");
			var value = Expression.Parameter(typeof(object), "value");
			var typedInstance = Expression.Convert(instance, instanceType);
			var typedValue = Expression.Convert(value, property.PropertyType);
			var call = Expression.Call(typedInstance, setMethod, typedValue);
			var lambda = Expression.Lambda<Action<object, object>>(call, new[] { instance, value });
			return (Action<object, object>)compiler.Invoke(lambda, null);
		}

		private class ReflectionInstantiator
		{
			private readonly ConstructorInfo _ctor;

			public ReflectionInstantiator(ConstructorInfo ctor)
			{
				_ctor = ctor;
			}

			public object Instantiate(object[] args)
			{
				return _ctor.Invoke(args);
			}
		}

		private class ReflectionPropertySetter
		{
			private readonly string _propertyName;

			public ReflectionPropertySetter(string propertyName)
			{
				_propertyName = propertyName;
			}

			public void SetPropertery(object instance, object value)
			{
				instance.GetType().GetProperty(_propertyName).SetValue(instance, value, null);
			}
		}

		public static Func<IContext, object> GetPropertyValueProvider(Type type, IContext context)
		{
			return GetValueProvider(true, type, context);
		}
	}
}
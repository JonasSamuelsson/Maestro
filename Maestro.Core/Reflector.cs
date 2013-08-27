using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro
{
	internal static class Reflector
	{
		private static bool TryGetExpressionCompiler(Type expressionType, out MethodInfo compile)
		{
			var methods = expressionType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			compile = methods.SingleOrDefault(x => x.Name == "Compile" && x.GetParameters().Length == 0);
			return compile != null;
		}

		public static Func<object[], object> GetInstantiator(ConstructorInfo ctor)
		{
			MethodInfo compiler;
			if (!TryGetExpressionCompiler(typeof(Expression<Func<object[], object>>), out compiler))
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
			if (!TryGetExpressionCompiler(typeof(Expression<Action<object, object>>), out compiler))
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
	}
}
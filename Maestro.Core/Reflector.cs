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

		public static Func<object> GetInstantiator(ConstructorInfo ctor)
		{
			throw new NotImplementedException();
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
using System;

namespace Maestro.Configuration
{
	public interface IPluginExpression
	{
		void Use(object instance);
		IFactoryInstanceExpression<object> Use(Func<object> factory);
		IFactoryInstanceExpression<object> Use(Func<IContext, object> factory);
		ITypeInstanceExpression<object> Use(Type type);

		void TryUse(object instance);
		IFactoryInstanceExpression<object> TryUse(Func<object> factory);
		IFactoryInstanceExpression<object> TryUse(Func<IContext, object> factory);
		ITypeInstanceExpression<object> TryUse(Type type);
	}

	public interface IPluginExpression<T>
	{
		void Use(T instance);
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceExpression<TInstance> Use<TInstance>() where TInstance : T;

		void TryUse(T instance);
		IFactoryInstanceExpression<TInstance> TryUse<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceExpression<TInstance> TryUse<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceExpression<TInstance> TryUse<TInstance>() where TInstance : T;
	}
}
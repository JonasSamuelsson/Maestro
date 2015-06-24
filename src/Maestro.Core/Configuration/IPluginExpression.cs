using System;

namespace Maestro.Configuration
{
	public interface IPluginExpression
	{
		void Use(object instance);
		IFactoryInstanceExpression<object> Use(Func<object> factory);
		IFactoryInstanceExpression<object> Use(Func<IContext, object> factory);
		ITypeInstanceExpression<object> Use(Type type);
	}

	public interface IPluginExpression<T>
	{
		void Use(T instance);
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceExpression<TInstance> Use<TInstance>() where TInstance : T;
	}
}
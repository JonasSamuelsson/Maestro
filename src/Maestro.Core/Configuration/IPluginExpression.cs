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
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory);
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory);
		ITypeInstanceExpression<TInstance> Use<TInstance>();
	}
}
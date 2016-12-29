using System;

namespace Maestro.Configuration
{
	public interface IDefaultPluginExpression : IPluginExpression
	{
		void Add(object instance);
		IFactoryInstanceExpression<object> Add(Func<object> factory);
		IFactoryInstanceExpression<object> Add(Func<IContext, object> factory);
		ITypeInstanceExpression<object> Add(Type type);
	}

	public interface IDefaultPluginExpression<T> : IPluginExpression<T>
	{
		void Add(T instance);
		IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceExpression<TInstance> Add<TInstance>() where TInstance : T;
	}
}
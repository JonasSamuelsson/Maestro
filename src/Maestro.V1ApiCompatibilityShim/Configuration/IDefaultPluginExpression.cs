using System;

namespace Maestro.Configuration
{
	public interface IDefaultPluginExpression : IPluginExpression
	{
		void Add(object instance);
		IFactoryInstanceConfigurator<object> Add(Func<object> factory);
		IFactoryInstanceConfigurator<object> Add(Func<IContext, object> factory);
		ITypeInstanceConfigurator<object> Add(Type type);
	}

	public interface IDefaultPluginExpression<T> : IPluginExpression<T>
	{
		void Add(T instance);
		IFactoryInstanceConfigurator<TInstance> Add<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceConfigurator<TInstance> Add<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceConfigurator<TInstance> Add<TInstance>() where TInstance : T;
	}
}
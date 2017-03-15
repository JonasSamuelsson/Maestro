using System;

namespace Maestro.Configuration
{
	public interface IPluginExpression
	{
		void Use(object instance);
		IFactoryInstanceConfigurator<object> Use(Func<object> factory);
		IFactoryInstanceConfigurator<object> Use(Func<IContext, object> factory);
		ITypeInstanceConfigurator<object> Use(Type type);

		void TryUse(object instance);
		IFactoryInstanceConfigurator<object> TryUse(Func<object> factory);
		IFactoryInstanceConfigurator<object> TryUse(Func<IContext, object> factory);
		ITypeInstanceConfigurator<object> TryUse(Type type);
	}

	public interface IPluginExpression<T>
	{
		void Use(T instance);
		IFactoryInstanceConfigurator<TInstance> Use<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceConfigurator<TInstance> Use<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceConfigurator<TInstance> Use<TInstance>() where TInstance : T;

		void TryUse(T instance);
		IFactoryInstanceConfigurator<TInstance> TryUse<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceConfigurator<TInstance> TryUse<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceConfigurator<TInstance> TryUse<TInstance>() where TInstance : T;
	}
}
using System;

namespace Maestro.Configuration
{
	public interface IServiceInstanceExpression
	{
		void Instance(object instance);
		IFactoryInstanceConfigurator<object> Factory(Func<object> factory);
		IFactoryInstanceConfigurator<object> Factory(Func<IContext, object> factory);
		ITypeInstanceConfigurator<object> Type(Type type);
		ITypeInstanceConfigurator<object> Self();
	}

	public interface IServiceInstanceExpression<T>
	{
		void Instance<TInstance>(TInstance instance) where TInstance : T;
		IFactoryInstanceConfigurator<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceConfigurator<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceConfigurator<TInstance> Type<TInstance>() where TInstance : T;
		ITypeInstanceConfigurator<T> Self();
	}
}
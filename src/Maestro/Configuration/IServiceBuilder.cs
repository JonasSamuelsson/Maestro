using System;

namespace Maestro.Configuration
{
	public interface IServiceBuilder
	{
		void Instance(object instance);
		IFactoryInstanceBuilder<object> Factory(Func<object> factory);
		IFactoryInstanceBuilder<object> Factory(Func<Context, object> factory);
		ITypeInstanceBuilder<object> Type(Type type);
		ITypeInstanceBuilder<object> Self();
	}

	public interface IServiceBuilder<T>
	{
		void Instance<TInstance>(TInstance instance) where TInstance : T;
		IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<Context, TInstance> factory) where TInstance : T;
		ITypeInstanceBuilder<TInstance> Type<TInstance>() where TInstance : T;
		ITypeInstanceBuilder<T> Self();
	}
}
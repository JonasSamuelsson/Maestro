using System;

namespace Maestro.Configuration
{
	public interface IServiceExpression
	{
		void Instance(object instance);
		IFactoryInstanceExpression<object> Factory(Func<object> factory);
		IFactoryInstanceExpression<object> Factory(Func<IContext, object> factory);
		ITypeInstanceExpression<object> Type(Type type);
		ITypeInstanceExpression<object> Self();
	}

	public interface IServiceExpression<T>
	{
		void Instance<TInstance>(TInstance instance) where TInstance : T;
		IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : T;
		IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory) where TInstance : T;
		ITypeInstanceExpression<TInstance> Type<TInstance>() where TInstance : T;
		ITypeInstanceExpression<T> Self();
	}
}
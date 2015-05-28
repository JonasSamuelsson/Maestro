using System;

namespace Maestro.Configuration
{
	public interface IFactoryExpression<T>
	{
		void Instance(T instance);
		IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<TInstance> factory);
		IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory);
		ITypeInstanceExpression<TInstance> Type<TInstance>();
	}
}
using System;

namespace Maestro.Configuration
{
	public interface ITypeInstanceConfigurator<TInstance> : IInstanceConfigurator<TInstance, ITypeInstanceConfigurator<TInstance>>
	{
		ITypeInstanceConfigurator<TInstance> CtorArg(string argName, object value);
		ITypeInstanceConfigurator<TInstance> CtorArg(string argName, Func<object> factory);
		ITypeInstanceConfigurator<TInstance> CtorArg(string argName, Func<IContext, object> factory);
		ITypeInstanceConfigurator<TInstance> CtorArg(string argName, Func<IContext, Type, object> factory);

		ITypeInstanceConfigurator<TInstance> CtorArg(Type argType, object value);
		ITypeInstanceConfigurator<TInstance> CtorArg(Type argType, Func<object> factory);
		ITypeInstanceConfigurator<TInstance> CtorArg(Type argType, Func<IContext, object> factory);

		ITypeInstanceConfigurator<TInstance> CtorArg<TValue>(TValue value);
		ITypeInstanceConfigurator<TInstance> CtorArg<TValue>(Func<TValue> factory);
		ITypeInstanceConfigurator<TInstance> CtorArg<TValue>(Func<IContext, TValue> factory);

		ITypeInstanceConfigurator<T> As<T>();
	}
}
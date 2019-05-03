using System;

namespace Maestro.Configuration
{
	public interface ITypeInstanceBuilder<TInstance> : IInstanceBuilder<TInstance, ITypeInstanceBuilder<TInstance>>
	{
		ITypeInstanceBuilder<TInstance> CtorArg(Type argType, object value);
		ITypeInstanceBuilder<TInstance> CtorArg(Type argType, Func<object> factory);
		ITypeInstanceBuilder<TInstance> CtorArg(Type argType, Func<Context, object> factory);

		ITypeInstanceBuilder<TInstance> CtorArg<TValue>(TValue value);
		ITypeInstanceBuilder<TInstance> CtorArg<TValue>(Func<TValue> factory);
		ITypeInstanceBuilder<TInstance> CtorArg<TValue>(Func<Context, TValue> factory);

		ITypeInstanceBuilder<T> As<T>();
	}
}
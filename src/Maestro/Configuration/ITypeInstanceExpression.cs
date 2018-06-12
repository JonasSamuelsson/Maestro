using System;

namespace Maestro.Configuration
{
	public interface ITypeInstanceExpression<TInstance> : IInstanceExpression<TInstance, ITypeInstanceExpression<TInstance>>
	{
		ITypeInstanceExpression<TInstance> CtorArg(Type argType, object value);
		ITypeInstanceExpression<TInstance> CtorArg(Type argType, Func<object> factory);
		ITypeInstanceExpression<TInstance> CtorArg(Type argType, Func<Context, object> factory);

		ITypeInstanceExpression<TInstance> CtorArg<TValue>(TValue value);
		ITypeInstanceExpression<TInstance> CtorArg<TValue>(Func<TValue> factory);
		ITypeInstanceExpression<TInstance> CtorArg<TValue>(Func<Context, TValue> factory);

		ITypeInstanceExpression<T> As<T>();
	}
}
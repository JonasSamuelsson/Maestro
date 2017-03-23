using System;

namespace Maestro.Configuration
{
	public interface ITypeInstanceExpression<TInstance> : IInstanceExpression<TInstance, ITypeInstanceExpression<TInstance>>
	{
		ITypeInstanceExpression<TInstance> CtorArg(string argName, object value);
		ITypeInstanceExpression<TInstance> CtorArg(string argName, Func<object> factory);
		ITypeInstanceExpression<TInstance> CtorArg(string argName, Func<IContext, object> factory);
		ITypeInstanceExpression<TInstance> CtorArg(string argName, Func<IContext, Type, object> factory);

		ITypeInstanceExpression<TInstance> CtorArg(Type argType, object value);
		ITypeInstanceExpression<TInstance> CtorArg(Type argType, Func<object> factory);
		ITypeInstanceExpression<TInstance> CtorArg(Type argType, Func<IContext, object> factory);

		ITypeInstanceExpression<TInstance> CtorArg<TValue>(TValue value);
		ITypeInstanceExpression<TInstance> CtorArg<TValue>(Func<TValue> factory);
		ITypeInstanceExpression<TInstance> CtorArg<TValue>(Func<IContext, TValue> factory);

		ITypeInstanceExpression<T> As<T>();
	}
}
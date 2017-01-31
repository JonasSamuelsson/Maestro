using System;

namespace Maestro.Configuration
{
	public interface ITypeInstanceExpression<T> : IInstanceExpression<T, ITypeInstanceExpression<T>>
	{
		ITypeInstanceExpression<T> CtorArg(string argName, object value);
		ITypeInstanceExpression<T> CtorArg(string argName, Func<object> factory);
		ITypeInstanceExpression<T> CtorArg(string argName, Func<IContext, object> factory);
		ITypeInstanceExpression<T> CtorArg(string argName, Func<IContext, Type, object> factory);

		ITypeInstanceExpression<T> CtorArg(Type argType, object value);
		ITypeInstanceExpression<T> CtorArg(Type argType, Func<object> factory);
		ITypeInstanceExpression<T> CtorArg(Type argType, Func<IContext, object> factory);

		ITypeInstanceExpression<T> CtorArg<TValue>(TValue value);
		ITypeInstanceExpression<T> CtorArg<TValue>(Func<TValue> factory);
		ITypeInstanceExpression<T> CtorArg<TValue>(Func<IContext, TValue> factory);
	}
}
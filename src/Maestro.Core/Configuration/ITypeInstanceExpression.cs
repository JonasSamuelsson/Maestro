using System;

namespace Maestro.Configuration
{
	public interface ITypeInstanceExpression<T> : IInstanceExpression<T, ITypeInstanceExpression<T>>
	{
		ITypeInstanceExpression<T> ConstructorDependency<TDependency>(TDependency dependency);
		ITypeInstanceExpression<T> ConstructorDependency(Type type, object dependency);
	}
}
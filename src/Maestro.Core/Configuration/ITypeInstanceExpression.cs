namespace Maestro.Configuration
{
	public interface ITypeInstanceExpression<T> : IInstanceExpression<T, ITypeInstanceExpression<T>>
	{
		ITypeInstanceExpression<T> CtorArg(string argName, object value);
	}
}
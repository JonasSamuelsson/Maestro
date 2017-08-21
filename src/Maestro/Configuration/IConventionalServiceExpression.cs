namespace Maestro.Configuration
{
	public interface IConventionalServiceExpression<T>
	{
		ITypeInstanceExpression<T> Use(string name = null);
		ITypeInstanceExpression<T> TryUse(string name = null);
		ITypeInstanceExpression<T> Add();
	}
}
namespace Maestro.Configuration
{
	public interface IConventionalServiceExpression<T>
	{
		ITypeInstanceExpression<T> Use();
		ITypeInstanceExpression<T> Use(string name);
		ITypeInstanceExpression<T> TryUse();
		ITypeInstanceExpression<T> TryUse(string name);
		ITypeInstanceExpression<T> Add();
	}
}
namespace Maestro.Configuration
{
	public interface IConventionalServiceTypeExpression<T>
	{
		IConventionalServiceExpression<T> BaseType { get; }
		IConventionalServiceExpression<T> Type { get; }
	}
}
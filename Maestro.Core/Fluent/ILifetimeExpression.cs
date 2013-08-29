namespace Maestro.Fluent
{
	public interface ILifetimeExpression<T>
	{
		ILifetimeSelector<T> Lifetime { get; }
	}
}
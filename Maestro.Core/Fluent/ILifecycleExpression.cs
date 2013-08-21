namespace Maestro.Fluent
{
	public interface ILifecycleExpression<T>
	{
		ILifecycleSelector<T> Lifecycle { get; }
	}
}
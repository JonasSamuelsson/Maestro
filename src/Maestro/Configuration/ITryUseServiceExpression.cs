namespace Maestro.Configuration
{
	public interface ITryUseServiceExpression
	{
		IServiceInstanceExpression TryUse { get; }
	}

	public interface ITryUseServiceExpression<T>
	{
		IServiceInstanceExpression<T> TryUse { get; }
	}
}
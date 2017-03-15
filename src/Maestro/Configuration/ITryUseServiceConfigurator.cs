namespace Maestro.Configuration
{
	public interface ITryUseServiceConfigurator
	{
		IServiceInstanceExpression TryUse { get; }
	}

	public interface ITryUseServiceConfigurator<T>
	{
		IServiceInstanceExpression<T> TryUse { get; }
	}
}
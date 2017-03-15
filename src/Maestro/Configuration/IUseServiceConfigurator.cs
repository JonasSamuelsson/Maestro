namespace Maestro.Configuration
{
	public interface IUseServiceConfigurator
	{
		IServiceInstanceExpression Use { get; }
	}

	public interface IUseServiceConfigurator<T>
	{
		IServiceInstanceExpression<T> Use { get; }
	}
}
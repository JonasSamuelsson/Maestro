namespace Maestro.Configuration
{
	public interface IAddServiceConfigurator
	{
		IServiceInstanceExpression Add { get; }
	}

	public interface IAddServiceConfigurator<T>
	{
		IServiceInstanceExpression<T> Add { get; }
	}
}
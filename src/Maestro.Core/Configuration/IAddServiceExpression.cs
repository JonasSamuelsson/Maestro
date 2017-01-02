namespace Maestro.Configuration
{
	public interface IAddServiceExpression
	{
		IServiceInstanceExpression Add { get; }
	}

	public interface IAddServiceExpression<T>
	{
		IServiceInstanceExpression<T> Add { get; }
	}
}
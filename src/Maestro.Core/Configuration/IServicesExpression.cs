namespace Maestro.Configuration
{
	public interface IServicesExpression
	{
		IServiceInstanceExpression Add { get; }
	}

	public interface IServicesExpression<T>
	{
		IServiceInstanceExpression<T> Add { get; }
	}
}
namespace Maestro.Configuration
{
	public interface IUseServiceExpression
	{
		IServiceInstanceExpression Use { get; }
	}

	public interface IUseServiceExpression<T>
	{
		IServiceInstanceExpression<T> Use { get; }
	}
}
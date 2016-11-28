namespace Maestro.Configuration
{
	public interface IServiceExpression
	{
		IServiceInstanceExpression Use { get; }
		IServiceInstanceExpression TryUse { get; }
	}

	public interface IServiceExpression<T>
	{
		IServiceInstanceExpression<T> Use { get; }
		IServiceInstanceExpression<T> TryUse { get; }
	}
}
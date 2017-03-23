namespace Maestro.Configuration
{
	public interface IAddServiceExpression
	{
		IInstanceKindSelector Add { get; }
	}

	public interface IAddServiceExpression<T>
	{
		IInstanceKindSelector<T> Add { get; }
	}
}
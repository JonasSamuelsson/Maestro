namespace Maestro.Configuration
{
	public interface ITryUseServiceExpression
	{
		IInstanceKindSelector TryUse { get; }
	}

	public interface ITryUseServiceExpression<T>
	{
		IInstanceKindSelector<T> TryUse { get; }
	}
}
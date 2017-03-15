namespace Maestro.Configuration
{
	public interface ITryUseServiceConfigurator
	{
		IInstanceKindSelector TryUse { get; }
	}

	public interface ITryUseServiceConfigurator<T>
	{
		IInstanceKindSelector<T> TryUse { get; }
	}
}
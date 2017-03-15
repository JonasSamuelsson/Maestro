namespace Maestro.Configuration
{
	public interface IUseServiceConfigurator
	{
		IInstanceKindSelector Use { get; }
	}

	public interface IUseServiceConfigurator<T>
	{
		IInstanceKindSelector<T> Use { get; }
	}
}
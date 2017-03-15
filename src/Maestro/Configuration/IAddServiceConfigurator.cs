namespace Maestro.Configuration
{
	public interface IAddServiceConfigurator
	{
		IInstanceKindSelector Add { get; }
	}

	public interface IAddServiceConfigurator<T>
	{
		IInstanceKindSelector<T> Add { get; }
	}
}
namespace Maestro.Configuration
{
	public interface IServiceConfigurator : IUseServiceConfigurator, ITryUseServiceConfigurator, IAddServiceConfigurator
	{
	}

	public interface IServiceConfigurator<T> : IUseServiceConfigurator<T>, ITryUseServiceConfigurator<T>, IAddServiceConfigurator<T>
	{
	}
}
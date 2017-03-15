namespace Maestro.Configuration
{
	public interface INamedServiceConfigurator : IUseServiceConfigurator, ITryUseServiceConfigurator
	{
	}

	public interface INamedServiceConfigurator<T> : IUseServiceConfigurator<T>, ITryUseServiceConfigurator<T>
	{
	}
}
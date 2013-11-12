using Maestro.Lifetimes;

namespace Maestro.Configuration
{
	public interface ILifetimeExpression<T>
	{
		T Transient();
		T RequestSingleton();
		T Singleton();
		T Custom<TLifetime>() where TLifetime : ILifetime, new();
		T Custom(ILifetime lifetime);
	}
}
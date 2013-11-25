using Maestro.Lifetimes;

namespace Maestro.Configuration
{
	public interface ILifetimeExpression<T>
	{
		/// <summary>
		/// Sets instance lifetime to transient.
		/// </summary>
		/// <returns></returns>
		T Transient();

		/// <summary>
		/// Sets instance lifetime to one instance per get-request to the container.
		/// </summary>
		/// <returns></returns>
		T RequestSingleton();

		/// <summary>
		/// Sets instance lifetime to singleton.
		/// </summary>
		/// <returns></returns>
		T Singleton();

		/// <summary>
		/// Use custom lifetime <typeparamref name="TLifetime"/>
		/// </summary>
		/// <typeparam name="TLifetime"></typeparam>
		/// <returns></returns>
		T Custom<TLifetime>() where TLifetime : ILifetime, new();

		/// <summary>
		/// Use custom lifetime <paramref name="lifetime"/>
		/// </summary>
		/// <param name="lifetime"></param>
		/// <returns></returns>
		T Custom(ILifetime lifetime);
	}
}
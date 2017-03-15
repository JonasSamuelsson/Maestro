namespace Maestro.Configuration
{
	public interface IFactoryInstanceConfigurator<TInstance> : IInstanceConfigurator<TInstance, IFactoryInstanceConfigurator<TInstance>>
	{
		/// <summary>
		/// Adds a func to execute against the instance.
		/// </summary>
		/// <returns></returns>
		IFactoryInstanceConfigurator<T> As<T>();
	}
}
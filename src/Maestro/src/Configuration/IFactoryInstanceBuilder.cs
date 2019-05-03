namespace Maestro.Configuration
{
	public interface IFactoryInstanceBuilder<TInstance> : IInstanceBuilder<TInstance, IFactoryInstanceBuilder<TInstance>>
	{
		/// <summary>
		/// Adds a func to execute against the instance.
		/// </summary>
		/// <returns></returns>
		IFactoryInstanceBuilder<T> As<T>();
	}
}
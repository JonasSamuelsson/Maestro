namespace Maestro.Configuration
{
	public interface IFactoryInstanceExpression<TInstance> : IInstanceExpression<TInstance, IFactoryInstanceExpression<TInstance>>
	{
		/// <summary>
		/// Adds a func to execute against the instance.
		/// </summary>
		/// <returns></returns>
		IFactoryInstanceExpression<T> As<T>();
	}
}
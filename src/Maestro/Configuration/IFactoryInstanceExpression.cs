using Maestro.Interceptors;

namespace Maestro.Configuration
{
	public interface IFactoryInstanceExpression<TInstance> : IInstanceExpression<TInstance, IFactoryInstanceExpression<TInstance>>
	{
		/// <summary>
		/// Adds a func to execute against the instance.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		IFactoryInstanceExpression<TInstanceOut> Intercept<TInstanceOut>(IInterceptor interceptor) where TInstanceOut : TInstance;
	}
}
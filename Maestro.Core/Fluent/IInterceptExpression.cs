using Maestro.Interceptors;

namespace Maestro.Fluent
{
	public interface IInterceptExpression<TInstance, TParent>
	{
		TParent InterceptWith(IInterceptor interceptor);
	}
}
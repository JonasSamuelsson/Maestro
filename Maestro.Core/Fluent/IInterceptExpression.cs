using Maestro.Interceptors;

namespace Maestro.Fluent
{
	public interface IInterceptExpression<TInstance, TParent>
	{
		TParent Intercept(IInterceptor interceptor);
	}
}
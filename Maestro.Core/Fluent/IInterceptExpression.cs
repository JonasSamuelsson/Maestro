using Maestro.Interceptors;

namespace Maestro.Fluent
{
	public interface IInterceptExpression<TParent>
	{
		TParent InterceptUsing(IInterceptor interceptor);
	}
}
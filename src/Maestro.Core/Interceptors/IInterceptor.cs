
namespace Maestro.Interceptors
{
	public interface IInterceptor
	{
		object Execute(object instance, IContext context);
	}

	public interface IInterceptor<TIn, TOut> : IInterceptor
	{
		TOut Execute(TIn instance, IContext context);
	}
}
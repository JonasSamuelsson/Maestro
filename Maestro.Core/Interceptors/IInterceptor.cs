namespace Maestro.Interceptors
{
	public interface IInterceptor
	{
		IInterceptor Clone();
		object Execute(object instance, IContext context);
	}

	public interface IInterceptor<TIn, TOut> : IInterceptor
	{
		TOut Execute(TIn instance, IContext context);
	}
}
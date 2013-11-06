namespace Maestro.Interceptors
{
	public abstract class InterceptorBase<TIn, TOut> : IInterceptor<TIn, TOut>
	{
		public abstract IInterceptor Clone();
		public abstract TOut Execute(TIn instance, IContext context);

		public object Execute(object instance, IContext context)
		{
			return Execute((TIn)instance, context);
		}
	}
}
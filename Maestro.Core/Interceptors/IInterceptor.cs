namespace Maestro.Interceptors
{
	public interface IInterceptor
	{
		IInterceptor Clone();
		object Execute(object instance, IContext context);
	}
}
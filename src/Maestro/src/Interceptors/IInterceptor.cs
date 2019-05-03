namespace Maestro.Interceptors
{
	public interface IInterceptor
	{
		object Execute(object instance, Context context);
	}
}
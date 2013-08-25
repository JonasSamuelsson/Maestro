namespace Maestro
{
	public interface IInterceptor : IPipelineItem<IInterceptor>
	{
		object Execute(object instance, IContext context);
	}
}
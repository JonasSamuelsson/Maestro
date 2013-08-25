namespace Maestro
{
	public interface ILifecycle : IPipelineItem<ILifecycle>
	{
		object Execute(IContext context, IPipeline pipeline);
	}
}
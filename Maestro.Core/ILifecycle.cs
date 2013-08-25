namespace Maestro
{
	public interface ILifecycle : IPipelineItem<ILifecycle>
	{
		object Process(IContext context, IPipeline pipeline);
	}
}
namespace Maestro
{
	public interface IPipelineItem
	{
		object Process(IContext context, IPipeline pipeline);
	}
}
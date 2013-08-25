namespace Maestro.Lifecycles
{
	public interface ILifecycle
	{
		ILifecycle Clone();
		object Execute(IContext context, IPipeline pipeline);
	}
}
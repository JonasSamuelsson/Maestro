namespace Maestro.Lifetimes
{
	public interface ILifetime
	{
		ILifetime Clone();
		object Execute(IContext context, IPipeline pipeline);
	}
}
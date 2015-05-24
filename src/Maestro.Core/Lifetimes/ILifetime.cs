namespace Maestro.Lifetimes
{
	public interface ILifetime
	{
		ILifetime Clone();
		object Execute(INextStep nextStep);
	}
}
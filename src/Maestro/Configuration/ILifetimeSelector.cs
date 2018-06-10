namespace Maestro.Configuration
{
	public interface ILifetimeSelector<TParent>
	{
		TParent Transient();
		TParent Scoped();
		TParent Singleton();
	}
}
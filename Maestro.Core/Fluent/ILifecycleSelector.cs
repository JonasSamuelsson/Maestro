namespace Maestro.Fluent
{
	public interface ILifecycleSelector<T>
	{
		T Transient();
		T Request();
		T Singleton();
		T Custom<TLifecycle>() where TLifecycle : ILifecycle, new();
		T Custom(ILifecycle lifecycle);
	}
}
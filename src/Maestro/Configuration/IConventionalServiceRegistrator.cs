namespace Maestro.Configuration
{
	public interface IConventionalServiceRegistrator<T>
	{
		IConventionalServiceBuilder<T> Add();
		IConventionalServiceBuilder<T> AddOrThrow();
		IConventionalServiceBuilder<T> TryAdd();
	}
}
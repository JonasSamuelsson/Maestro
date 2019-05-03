namespace Maestro.Configuration
{
	public interface IConventionalServiceRegistrator<T>
	{
		IConventionalServiceBuilder<T> Add();
		IConventionalServiceBuilder<T> Add(ServiceType serviceType);
		IConventionalServiceBuilder<T> AddOrThrow();
		IConventionalServiceBuilder<T> AddOrThrow(ServiceType serviceType);
		IConventionalServiceBuilder<T> TryAdd();
		IConventionalServiceBuilder<T> TryAdd(ServiceType serviceType);
	}
}
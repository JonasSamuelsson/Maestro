namespace Maestro.Configuration
{
	public interface IConventionalServiceBuilder<T> : ITypeInstanceBuilder<T>
	{
		ITypeInstanceBuilder<T> Named(string name);
	}
}
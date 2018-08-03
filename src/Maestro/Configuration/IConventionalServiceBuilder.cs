namespace Maestro.Configuration
{
	public interface IConventionalServiceBuilder<T>
	{
		ITypeInstanceBuilder<T> Use();
		ITypeInstanceBuilder<T> Use(string name);
		ITypeInstanceBuilder<T> TryUse();
		ITypeInstanceBuilder<T> TryUse(string name);
		ITypeInstanceBuilder<T> Add();
	}
}
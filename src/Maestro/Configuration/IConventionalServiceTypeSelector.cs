namespace Maestro.Configuration
{
	public interface IConventionalServiceTypeSelector<T>
	{
		IConventionalTypeInstanceRegistrator<T> BaseType { get; }
		IConventionalTypeInstanceRegistrator<T> Type { get; }
	}
}
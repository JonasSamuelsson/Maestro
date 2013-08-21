namespace Maestro.Fluent
{
	public interface ITypeInstanceBuilder
	{
	}

	public interface ITypeInstanceBuilder<T>:ILifecycleExpression<ITypeInstanceBuilder<T>> 
	{
	}
}
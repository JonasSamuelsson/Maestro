namespace Maestro.Fluent
{
	public interface ILambdaInstanceBuilder<T> : IOnCreateExpression<ILambdaInstanceBuilder<T>>,
		ILifecycleExpression<ILambdaInstanceBuilder<T>>,
		IOnActivateExpression<ILambdaInstanceBuilder<T>>
	{
	}
}
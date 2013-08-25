namespace Maestro.Fluent
{
	public interface ILambdaInstanceBuilder<TInstance> : IOnCreateExpression<ILambdaInstanceBuilder<TInstance>>,
		ILifecycleExpression<ILambdaInstanceBuilder<TInstance>>,
		IOnActivateExpression<ILambdaInstanceBuilder<TInstance>>
	{
	}
}
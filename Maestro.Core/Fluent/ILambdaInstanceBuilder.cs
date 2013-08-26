namespace Maestro.Fluent
{
	public interface ILambdaInstanceBuilder<TInstance> : IOnCreateExpression<TInstance, ILambdaInstanceBuilder<TInstance>>,
		ILifecycleExpression<ILambdaInstanceBuilder<TInstance>>,
		IOnActivateExpression<TInstance, ILambdaInstanceBuilder<TInstance>>
	{
	}
}
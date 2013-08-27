namespace Maestro.Fluent
{
	public interface IDefaultsExpression
	{
		ILifecycleSelector<IDefaultsExpression> Lifecycle { get; }
	}
}
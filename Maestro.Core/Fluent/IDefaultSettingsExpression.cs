namespace Maestro.Fluent
{
	public interface IDefaultSettingsExpression
	{
		ILifecycleSelector<IDefaultSettingsExpression> Lifecycle { get; }
		IDefaultFilterExpression Filters { get; }
	}
}
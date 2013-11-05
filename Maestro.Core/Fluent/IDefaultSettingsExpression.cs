namespace Maestro.Fluent
{
	public interface IDefaultSettingsExpression
	{
		ILifetimeExpression<IDefaultSettingsExpression> Lifetime { get; }
		IDefaultFilterExpression Filters { get; }
	}
}
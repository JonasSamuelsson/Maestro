namespace Maestro.Configuration
{
	public interface IDefaultSettingsExpression
	{
		ILifetimeExpression<IDefaultSettingsExpression> Lifetime { get; }
		IDefaultFilterExpression Filters { get; }
	}
}
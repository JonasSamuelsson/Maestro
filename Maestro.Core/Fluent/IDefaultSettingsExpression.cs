namespace Maestro.Fluent
{
	public interface IDefaultSettingsExpression
	{
		ILifetimeSelector<IDefaultSettingsExpression> Lifetime { get; }
		IDefaultFilterExpression Filters { get; }
	}
}
namespace Maestro.Configuration
{
	public interface IDefaultSettingsExpression
	{
		/// <summary>
		/// Uses to configure the default lifetime to use if one isn't explicitly defined.
		/// </summary>
		ILifetimeExpression<IDefaultSettingsExpression> Lifetime { get; }

		/// <summary>
		/// Used to configure global filters.
		/// </summary>
		IDefaultFilterExpression Filters { get; }
	}
}
namespace Maestro.Configuration
{
	public interface IDefaultsExpression
	{
		IDefaultLifetimeSelector Lifetime { get; }
		GetServicesOrder GetServicesOrder { get; set; }
	}
}
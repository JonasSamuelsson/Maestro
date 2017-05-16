namespace Maestro.Configuration
{
	public interface IConfigExpression
	{
		IDefaultLifetimeSelector DefaultLifetime { get; }
		GetServicesOrder GetServicesOrder { get; set; }
	}
}
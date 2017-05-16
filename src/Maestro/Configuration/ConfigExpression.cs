namespace Maestro.Configuration
{
	internal class ConfigExpression : IConfigExpression
	{
		private readonly Config _config;

		internal ConfigExpression(Config config)
		{
			_config = config;
		}

		public IDefaultLifetimeSelector DefaultLifetime => new DefaultLifetimeSelector(lifetimeFactory => _config.LifetimeFactory = lifetimeFactory);

		public GetServicesOrder GetServicesOrder
		{
			get { return _config.GetServicesOrder; }
			set { _config.GetServicesOrder = value; }
		}
	}
}
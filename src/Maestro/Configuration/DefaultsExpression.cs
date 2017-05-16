namespace Maestro.Configuration
{
	internal class DefaultsExpression : IDefaultsExpression
	{
		private readonly DefaultSettings _defaultSettings;

		internal DefaultsExpression(DefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
		}

		public IDefaultLifetimeSelector Lifetime => new DefaultLifetimeSelector(lifetimeFactory => _defaultSettings.LifetimeFactory = lifetimeFactory);

		public GetServicesOrder GetServicesOrder
		{
			get { return _defaultSettings.GetServicesOrder; }
			set { _defaultSettings.GetServicesOrder = value; }
		}
	}
}
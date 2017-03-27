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
	}
}
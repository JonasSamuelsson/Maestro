namespace Maestro.Configuration
{
	public class DefaultsExpression
	{
		private readonly DefaultSettings _defaultSettings;

		internal DefaultsExpression(DefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
		}

		public DefaultLifetimeSelector Lifetime => new DefaultLifetimeSelector(lifetimeFactory => _defaultSettings.LifetimeFactory = lifetimeFactory);
	}
}
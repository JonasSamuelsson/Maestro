namespace Maestro.Configuration
{
	public class DefaultsConfigurator
	{
		private readonly DefaultSettings _defaultSettings;

		internal DefaultsConfigurator(DefaultSettings defaultSettings)
		{
			_defaultSettings = defaultSettings;
		}

		public DefaultLifetimeSelector Lifetime => new DefaultLifetimeSelector(lifetimeFactory => _defaultSettings.LifetimeFactory = lifetimeFactory);
	}
}
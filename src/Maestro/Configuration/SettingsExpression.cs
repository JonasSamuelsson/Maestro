namespace Maestro.Configuration
{
	public class SettingsExpression
	{
		private readonly ContainerSettings _containerSettings;

		internal SettingsExpression(ContainerSettings containerSettings)
		{
			_containerSettings = containerSettings;
		}

		public DefaultLifetimeExpression DefaultLifetime => new DefaultLifetimeExpression(_containerSettings);

		public GetServicesOrder GetServicesOrder
		{
			get => _containerSettings.GetServicesOrder;
			set => _containerSettings.GetServicesOrder = value;
		}
	}
}
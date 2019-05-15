using System;

namespace Maestro.Configuration
{
	public class ConfigurationPolicyAttribute : Attribute
	{
		public ConfigurationPolicyAttribute(Type policyType)
		{
			PolicyType = policyType;
		}

		internal Type PolicyType { get; }
	}
}
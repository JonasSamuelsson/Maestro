using System;

namespace Maestro.Configuration
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ServiceRegistrationPolicyAttribute : Attribute
	{
		public Type PolicyType { get; }

		public ServiceRegistrationPolicyAttribute(Type policyType)
		{
			PolicyType = policyType;
		}
	}
}
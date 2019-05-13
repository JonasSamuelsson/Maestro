using Maestro.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Conventions
{
	internal class ServiceRegistrationPolicyConvention : IConvention
	{
		public void Process(IEnumerable<Type> types, IContainerBuilder builder)
		{
			types.ForEach(type => type
				.GetCustomAttributes<ServiceRegistrationPolicyAttribute>(true)
				.Select(x => (IServiceRegistrationPolicy)Activator.CreateInstance(x.PolicyType))
				.ForEach(x => x.Register(type, builder))
			);
		}
	}
}
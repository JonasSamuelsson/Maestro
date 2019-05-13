using System;

namespace Maestro.Configuration
{
	public interface IServiceRegistrationPolicy
	{
		void Register(Type type, IContainerBuilder builder);
	}
}
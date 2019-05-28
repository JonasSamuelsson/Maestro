using System;

namespace Maestro.Configuration
{
	public interface IServiceRegistrationPolicy
	{
		void Execute(Type type, IContainerBuilder builder);
	}
}
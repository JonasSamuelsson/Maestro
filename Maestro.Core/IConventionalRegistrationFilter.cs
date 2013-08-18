using System;

namespace Maestro
{
	public interface IConventionalRegistrationFilter
	{
		bool IsMatch(Type type);
	}
}
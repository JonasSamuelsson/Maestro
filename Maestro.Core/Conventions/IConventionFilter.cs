using System;

namespace Maestro.Conventions
{
	public interface IConventionFilter
	{
		bool IsMatch(Type type);
	}
}
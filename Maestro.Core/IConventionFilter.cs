using System;

namespace Maestro
{
	public interface IConventionFilter
	{
		bool IsMatch(Type type);
	}
}
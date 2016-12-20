using System;

namespace Maestro.Conventions
{
	public interface IFilter
	{
		bool IsMatch(Type type);
	}
}
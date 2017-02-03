using System;

namespace Maestro
{
	public interface IFilter
	{
		bool IsMatch(Type type);
	}
}
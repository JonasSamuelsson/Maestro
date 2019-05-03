using System;

namespace Maestro.Utils
{
	internal static class HashCode
	{
		internal static int Compute(Type type, string s)
		{
			var hash = 12347;
			hash = (hash * 1259) ^ type.GetHashCode();
			hash = (hash * 1259) ^ s.GetHashCode();
			return hash;
		}
	}
}

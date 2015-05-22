using System;

namespace Maestro
{
	public class DependencyActivationException : Exception
	{
		public DependencyActivationException(string message) : base(message) { }
		public DependencyActivationException(string message, Exception innerException) : base(message, innerException) { }
	}
}
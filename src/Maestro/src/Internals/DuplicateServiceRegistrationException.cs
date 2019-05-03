using System;

namespace Maestro.Internals
{
	internal class DuplicateServiceRegistrationException : Exception
	{
		public DuplicateServiceRegistrationException(string message) : base(message) { }
	}
}
using Maestro.Tests.Core.Internals;

namespace Maestro.Tests
{
	public class Program
	{
		static void Main()
		{
			new ConstructorProviderTests().Should_get_instance_constructors();
		}
	}
}
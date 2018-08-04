using Shouldly;
using Xunit;

namespace Maestro.Tests.OldTests
{
	public class default_type_instance_without_dependencies
	{
		[Fact]
		public void instantiate_type_instance()
		{
			new Container(x => x.Add(typeof(object)).Type(typeof(object)))
				.GetService(typeof(object))
				.ShouldBeOfType<object>();
		}
	}
}
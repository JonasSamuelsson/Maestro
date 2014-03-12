using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class default_type_instance_without_dependencies
	{
		[Fact]
		public void instantiate_type_instance()
		{
			new Container(x => x.For(typeof(object)).Use(typeof(object)))
				.Get(typeof(object))
				.Should().BeOfType<object>();
		}
	}
}
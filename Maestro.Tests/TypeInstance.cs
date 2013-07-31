using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class TypeInstance
	{
		[Fact]
		public void instantiate_type_instance()
		{
			new Container(x => x.Default(typeof(object)).Type(typeof(object)))
				.Get(typeof(object))
				.Should().BeOfType<object>();
		}
	}
}
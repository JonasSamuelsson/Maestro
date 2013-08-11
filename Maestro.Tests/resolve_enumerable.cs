using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_enumerable
	{
		[Fact]
		public void should_get_empty_enumerable_if_type_is_not_registered()
		{
			var enumerable = new Container().GetAll<object>();

			enumerable.Should().BeEmpty();
		}
	}
}
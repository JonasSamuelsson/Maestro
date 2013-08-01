using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Maestro.Tests
{
	public class named_type_instance_without_dependencies
	{
		[Fact]
		public void instantiate_type_instance()
		{
			var name = "foo";
			var container = new Container(x => x.Add<object>(name).Type<List<int>>());
			
			var o = container.Get<object>(name);
			
			o.Should().NotBeNull();
			o.Should().BeOfType<List<int>>();
		}
	}
}
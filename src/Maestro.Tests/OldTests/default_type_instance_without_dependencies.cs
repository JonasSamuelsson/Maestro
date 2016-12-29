using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class default_type_instance_without_dependencies
	{
		[Fact]
		public void instantiate_type_instance()
		{
			new Container(x => x.Service(typeof(object)).Use.Type(typeof(object)))
				.GetService(typeof(object))
				.ShouldBeOfType<object>();
		}
	}
}
using Shouldly;
using Xunit;

namespace Maestro.Tests.TypeFactoryResolvers
{
	public class AutoResolveFilters_tests
	{
		[Fact]
		public void should_get_instance_if_no_filter_is_provided()
		{
			new Container()
				.TryGetService<Target>(out _)
				.ShouldBeTrue();
		}

		[Fact]
		public void should_get_instance_if_matching_filter_is_provided()
		{
			new Container(x => x.AutoResolveFilters.Add(_ => true))
				.TryGetService<Target>(out _)
				.ShouldBeTrue();
		}

		[Fact]
		public void should_not_get_instance_if_no_matching_filter_is_provided()
		{
			new Container(x => x.AutoResolveFilters.Add(_ => false))
				.TryGetService<Target>(out _)
				.ShouldBeFalse();
		}

		private class Target { }
	}
}
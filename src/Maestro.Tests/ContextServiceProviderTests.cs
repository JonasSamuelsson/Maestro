using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class ContextServiceProviderTests
	{
		[Fact]
		public void ShouldGetService()
		{
			var container = new Container(x =>
			{
				x.Add<Root>().Factory(ctx => new Root { Dependency = ctx.ToServiceProvider().GetService(typeof(object)) });
				x.Add<object>().Instance("success");
			});

			container.GetService<Root>().Dependency.ShouldBe("success");
		}

		class Root
		{
			public object Dependency { get; set; }
		}
	}
}
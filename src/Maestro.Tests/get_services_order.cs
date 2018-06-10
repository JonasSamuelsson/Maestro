using System.Linq;
using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class get_services_order
	{
		[Fact]
		public void should_return_services_in_the_same_order_they_were_registered()
		{
			var instance = new TestType<int>();
			var container = new Container(x =>
			{
				x.Add<ITestType<int>>().Instance(instance);
				x.Add(typeof(ITestType<>)).Type(typeof(TestType<>));
				x.Add<ITestType<int>>().Type<TestTypeOfInt>();
				x.Settings.GetServicesOrder = GetServicesOrder.Ordered;
			});

			var instances = container.GetServices<ITestType<int>>().ToList();

			instances[0].ShouldBe(instance);
			instances[1].ShouldBeOfType<TestType<int>>();
			instances[2].ShouldBeOfType<TestTypeOfInt>();
		}

		interface ITestType<T> { }
		class TestType<T> : ITestType<T> { }
		class TestTypeOfInt : ITestType<int> { }
	}
}
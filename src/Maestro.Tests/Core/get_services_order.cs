using System.Linq;
using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class get_services_order
	{
		[Fact]
		public void should_return_services_in_the_same_order_they_were_registered()
		{
			var instance = new TestType<int>();
			var container = new Container(x =>
			{
				x.For<ITestType<int>>().Add.Instance(instance);
				x.For(typeof(ITestType<>)).Add.Type(typeof(TestType<>));
				x.For<ITestType<int>>().Add.Type<TestTypeOfInt>();
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
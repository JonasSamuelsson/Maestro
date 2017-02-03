using System.Linq;
using Maestro.CommonServiceLocator;
using Shouldly;
using Xunit;

namespace Maestro.Tests.CommonServiceLocator
{
	public class adapter_tests
	{
		[Fact]
		public void should_get_instance()
		{
			var container = new Container(x => x.For<object>().Use.Instance("success"));
			var adapter = new MaestroServiceLocator(container);
			adapter.GetInstance<object>().ShouldBe("success");
		}
	
		[Fact]
		public void should_get_instances()
		{
			var container = new Container(x => x.For<object>().Use.Instance("success"));
			var adapter = new MaestroServiceLocator(container);
			adapter.GetAllInstances<object>().Single().ShouldBe("success");
		}
	}
}
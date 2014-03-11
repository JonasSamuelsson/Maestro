using Shouldly;
using System.Linq;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class add_concrete_sub_classes_convention
	{
		[Fact]
		public void should_register_concrete_sub_classes_of_provided_type()
		{
			var types = new[] { typeof(Implementation1), typeof(Implementation2) };
			var container = new Container(x => x.Scan.Types(types).AddConcreteSubClassesOf<IBaseType>());

			var instances = container.GetAll<IBaseType>().ToList();

			instances.Count.ShouldBe(2);
			instances.OfType<Implementation1>().Count().ShouldBe(1);
			instances.OfType<Implementation2>().Count().ShouldBe(1);
		}

		[Fact]
		public void should_support_instance_configuration()
		{
			var types = new[] { typeof(Implementation1) };

			var container1 = new Container(x => x.Scan.Types(types).AddConcreteSubClassesOf(typeof(IBaseType), y => y.Lifetime.Singleton()));
			var instance1 = container1.GetAll<IBaseType>().Single();
			var instance2 = container1.GetAll<IBaseType>().Single();
			instance1.ShouldBe(instance2);

			var container2 = new Container(x => x.Scan.Types(types).AddConcreteSubClassesOf<IBaseType>(y => y.Lifetime.Singleton()));
			var instance3 = container2.GetAll<IBaseType>().Single();
			var instance4 = container2.GetAll<IBaseType>().Single();
			instance3.ShouldBe(instance4);
		}

		private interface IBaseType { }
		private class Implementation1 : IBaseType { }
		private class Implementation2 : IBaseType { }
	}
}
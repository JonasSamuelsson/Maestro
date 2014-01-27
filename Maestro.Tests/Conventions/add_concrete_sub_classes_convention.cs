using System.Linq;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class add_concrete_sub_classes_convention
	{
		[Fact]
		public void should_register_concrete_sub_classes_of_provided_type()
		{
			var types = GetType().GetNestedTypes(BindingFlags.NonPublic);
			var container = new Container(x => x.Scan.Types(types).AddConcreteSubClassesOf<IBaseType>());

			var instances = container.GetAll<IBaseType>().ToList();

			instances.Count.ShouldBe(2);
			instances.OfType<Implementation1>().Count().ShouldBe(1);
			instances.OfType<Implementation2>().Count().ShouldBe(1);
		}

		private interface IBaseType { }
		private class Implementation1 : IBaseType { }
		private class Implementation2 : IBaseType { }
	}
}
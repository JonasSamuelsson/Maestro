using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Lifetimes
{
	public class transient_lifetime
	{
		[Fact]
		public void transient_should_be_the_default_lifetime_and_always_result_in_a_new_instance()
		{
			var container = new Container(x =>
			                              {
				                              x.For<object>().Use<object>();
				                              x.For<Parent>().Use<Parent>();
			                              });

			var instance = container.Get<Parent>();

			instance.Object1.Should().NotBe(instance.Object2);
		}

		private class Parent
		{
			public Parent(object object1, object object2)
			{
				Object1 = object1;
				Object2 = object2;
			}

			public object Object1 { get; private set; }
			public object Object2 { get; private set; }
		}
	}
}
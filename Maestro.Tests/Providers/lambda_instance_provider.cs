using FluentAssertions;
using Xunit;

namespace Maestro.Tests.Providers
{
	public class lambda_instance_provider
	{
		[Fact]
		public void should_delegate_instantiation_to_provided_lambda()
		{
			var o = new object();
			var instantiator = new Instantiator(o);

			var container = new Container(x => x.For<object>().Use(instantiator.Get));
			var instance = container.Get<object>();

			instance.Should().NotBeNull();
			instantiator.Executed.Should().BeTrue();
		}

		private class Instantiator
		{
			private readonly object _o;

			public Instantiator(object o)
			{
				_o = o;
			}

			public bool Executed { get; private set; }

			public object Get()
			{
				Executed = true;
				return _o;
			}
		}
	}
}
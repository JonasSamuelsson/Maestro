using FluentAssertions;
using System;
using Xunit;

namespace Maestro.Tests
{
	public class property_injection
	{
		[Fact]
		public void set_ok()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty("Object"));
			var instance = container.Get<Foobar>();

			instance.Object.Should().NotBeNull();
		}

		[Fact]
		public void set_nok_1()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty("missing property"));

			container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		}

		//[Fact]
		//public void set_nok_2()
		//{
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty(y => y.Disposable));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		//}

		//[Fact]
		//public void try_set_ok()
		//{
		//	var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty(y => y.Disposable));

		//	container.Invoking(x => x.Get<Foobar>()).ShouldNotThrow();
		//}

		[Fact]
		public void try_set_nok()
		{
			var container = new Container(x => x.For<Foobar>().Use<Foobar>().OnCreate.SetProperty("missing property"));

			container.Invoking(x => x.Get<Foobar>()).ShouldThrow<ActivationException>();
		}

		private class Foobar
		{
			public object Object { get; set; }
			public IDisposable Disposable { get; set; }
		}
	}
}
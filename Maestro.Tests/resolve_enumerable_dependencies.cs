using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_enumerable_dependencies
	{
		[Fact]
		public void should_use_all_registered_instences_of_the_enumerated_type()
		{
			var container = new Container(x =>
			{
				x.Default<Foobar>().Type<Foobar>();
				x.Add<object>().Type<EventArgs>();
				x.Add<object>().Type<Exception>();
			});

			var foobar = container.Get<Foobar>();

			foobar.Objects.Should().HaveCount(2);
			foobar.Objects.Should().Contain(x => x.GetType() == typeof(EventArgs));
			foobar.Objects.Should().Contain(x => x.GetType() == typeof(Exception));
		}

		[Fact]
		public void should_use_empty_enumerable_if_enumerated_type_is_not_registered()
		{
			var container = new Container(x => x.Default<Foobar>().Type<Foobar>());

			var foobar = container.Get<Foobar>();

			foobar.Objects.Should().HaveCount(0);
		}

		private class Foobar
		{
			public Foobar(IEnumerable<object> objects) { Objects = objects; }
			public IEnumerable<object> Objects { get; private set; }
		}
	}
}
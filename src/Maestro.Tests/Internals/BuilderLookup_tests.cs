using System;
using System.Collections.Generic;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class BuilderLookup_tests
	{
		[Fact]
		public void Should_get_builder_by_key()
		{
			var expectedBuilder = new Builder();
			var lookup = new BuilderLookup();
			lookup.Add("key", expectedBuilder);
			IBuilder builder;
			lookup.TryGet("key", out builder).ShouldBe(true);
			builder.ShouldBe(expectedBuilder);
		}

		[Fact]
		public void Should_get_builders_by_key()
		{
			var builder1 = new Builder();
			var builder2 = new Builder();
			var lookup = new BuilderLookup();
			lookup.Add("key", new[] { builder1, builder2 });
			IEnumerable<IBuilder> builders;
			lookup.TryGet("key", out builders).ShouldBe(true);
			builders.ShouldBe(new[] { builder1, builder2 });
		}

		[Fact]
		public void Should_throw_when_getting_single_builder_from_enumerable()
		{
			var lookup = new BuilderLookup();
			lookup.Add("key", new[] { new Builder(), new Builder() });
			IBuilder builder;
			Should.Throw<InvalidOperationException>(() => lookup.TryGet("key", out builder));
		}

		class Builder : IBuilder
		{
			public object Execute()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
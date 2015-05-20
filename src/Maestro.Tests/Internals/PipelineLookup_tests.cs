using System;
using System.Collections.Generic;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class PipelineLookup_tests
	{
		[Fact]
		public void Should_get_builder_by_key()
		{
			var expectedPipeline = new Pipeline();
			var lookup = new PipelineLookup();

			lookup.Add("key", expectedPipeline);

			Maestro.Internals.IPipeline pipeline;
			lookup.TryGet("key", out pipeline).ShouldBe(true);
			pipeline.ShouldBe(expectedPipeline);
		}

		[Fact]
		public void Should_get_builders_by_key()
		{
			var pipeline1 = new Pipeline();
			var pipeline2 = new Pipeline();
			var lookup = new PipelineLookup();

			lookup.Add("key", new[] { pipeline1, pipeline2 });

			IEnumerable<Maestro.Internals.IPipeline> pipelines;
			lookup.TryGet("key", out pipelines).ShouldBe(true);
			pipelines.ShouldBe(new[] { pipeline1, pipeline2 });
		}

		[Fact]
		public void Should_throw_when_getting_single_builder_from_enumerable()
		{
			var lookup = new PipelineLookup();

			lookup.Add("key", new[] { new Pipeline(), new Pipeline() });

			Maestro.Internals.IPipeline pipeline;
			Should.Throw<InvalidOperationException>(() => lookup.TryGet("key", out pipeline));
		}
	}
}
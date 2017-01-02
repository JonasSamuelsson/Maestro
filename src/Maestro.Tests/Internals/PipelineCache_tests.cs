using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Internals;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Internals
{
	public class PipelineCache_tests
	{
		[Fact]
		public void should_not_get_pipeline_if_it_has_not_been_added()
		{
			var cache = new PipelineCache<string>();

			IPipeline pipeline;
			cache.TryGet("key not found", out pipeline).ShouldBe(false);
			pipeline.ShouldBeNull();
		}

		[Fact]
		public void should_not_get_pipelines_if_they_have_not_been_added()
		{
			var cache = new PipelineCache<string>();

			IEnumerable<IPipeline> pipelines;
			cache.TryGet("key not found", out pipelines).ShouldBe(false);
			pipelines.ShouldBeNull();
		}

		[Fact]
		public void should_get_pipeline_from_single_pipeline_collection()
		{
			var expectedPipeline = new Pipeline();

			var cache = new PipelineCache<string>();

			cache.Add("key", expectedPipeline);

			IPipeline pipeline;
			cache.TryGet("key", out pipeline).ShouldBe(true);
			pipeline.ShouldBe(expectedPipeline);
		}

		[Fact]
		public void should_get_pipelines_from_single_pipeline_collection()
		{
			var pipeline = new Pipeline();

			var cache = new PipelineCache<string>();

			cache.Add("key", new[] { pipeline });

			IEnumerable<IPipeline> pipelines;
			cache.TryGet("key", out pipelines).ShouldBe(true);
			pipelines.ShouldBe(new[] { pipeline });
		}

		[Fact]
		public void get_pipeline_from_multi_pipeline_collection_should_throw()
		{
			var cache = new PipelineCache<string>();

			cache.Add("key", new[] { new Pipeline(), new Pipeline() });

			IPipeline pipeline;
			Should.Throw<InvalidOperationException>(() => cache.TryGet("key", out pipeline));
		}

		[Fact]
		public void should_get_pipelines_from_multi_pipeline_collection()
		{
			var pipeline1 = new Pipeline();
			var pipeline2 = new Pipeline();
			var cache = new PipelineCache<string>();

			cache.Add("key", new[] { pipeline1, pipeline2 });

			IEnumerable<IPipeline> pipelines;
			cache.TryGet("key", out pipelines).ShouldBe(true);
			pipelines.ShouldBe(new[] { pipeline1, pipeline2 });
		}

		[Fact]
		public void clear_should_empty_cache()
		{
			var cache = new PipelineCache<string>();

			cache.Add("key", new Pipeline());

			IEnumerable<IPipeline> pipelines;

			cache.TryGet("key", out pipelines).ShouldBe(true);
			pipelines.Count().ShouldBe(1);

			cache.Clear();

			cache.TryGet("key", out pipelines).ShouldBe(false);
			pipelines.ShouldBeNull();

			cache.Add("key", new Pipeline());

			cache.TryGet("key", out pipelines).ShouldBe(true);
			pipelines.Count().ShouldBe(1);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;
using Maestro.Conventions;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class default_settings
	{
		[Fact]
		public void configured_filter_should_be_used_in_conventional_registrations()
		{
			var types = new[] { typeof(object), typeof(int) };
			var convention = new Convention();
			new Container(x =>
							  {
								  x.Default.Filters.Add(t => !t.Name.Contains("n"));
								  x.Scan(_ => _.Types(types).With(convention));
							  });
			convention.Types.ShouldBe(new[] { typeof(object) });
		}

		public class Convention : IConvention
		{
			public IEnumerable<Type> Types { get; private set; }

			public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
			{
				Types = types.ToList();
			}
		}
	}
}
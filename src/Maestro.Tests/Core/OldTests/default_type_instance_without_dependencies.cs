﻿using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class default_type_instance_without_dependencies
	{
		[Fact]
		public void instantiate_type_instance()
		{
			new Container(x => x.For(typeof(object)).Use.Type(typeof(object)))
				.GetService(typeof(object))
				.ShouldBeOfType<object>();
		}
	}
}
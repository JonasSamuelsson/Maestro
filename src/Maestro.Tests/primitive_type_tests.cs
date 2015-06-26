﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public abstract class primitive_type_tests
	{
		protected object ExpectedValue;
		protected Type PrimitiveType;

		[Fact]
		public void should_resolve_registered_instance()
		{
			var container = new Container(x => x.For(PrimitiveType).Use(ExpectedValue));

			var instance = container.Get(PrimitiveType);

			instance.ShouldBe(ExpectedValue);
		}

		[Fact]
		public void should_not_resolve_unregistered_instance()
		{
			var container = new Container();

			Should.Throw<ActivationException>(() => container.Get(PrimitiveType));
		}

		[Fact]
		public void should_resolve_all_with_no_instances_registered()
		{
			var container = new Container();

			var instance = container.GetAll(PrimitiveType);

			instance.ShouldBe(new object[] { });
		}

		[Fact]
		public void should_resolve_all_with_registered_instance()
		{
			var container = new Container(x => x.For(PrimitiveType).Use(ExpectedValue));

			var instance = container.GetAll(PrimitiveType);

			instance.ShouldBe(new[] { ExpectedValue });
		}

		public class resolve_struct : primitive_type_tests
		{
			public resolve_struct()
			{
				ExpectedValue = 1;
				PrimitiveType = typeof(int);
			}
		}

		public class resolve_string : primitive_type_tests
		{
			public resolve_string()
			{
				ExpectedValue = "foobar";
				PrimitiveType = typeof(string);
			}
		}

		public class resolve_object : primitive_type_tests
		{
			public resolve_object()
			{
				ExpectedValue = new object();
				PrimitiveType = typeof(object);
			}
		}
	}
}

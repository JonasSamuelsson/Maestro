using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class OptionalParametersTests
	{
		[Fact]
		public void ShouldHandleTypesWithOptionalConstructorParameters()
		{
			var container = new Container(x => x.Add<Service>().Self());

			var service = container.GetService<Service>();

			service.Object.ShouldBeNull();
			service.Text.ShouldBe("success");
		}

		private class Service
		{
			public Service(object @object = null, string text = "success")
			{
				Object = @object;
				Text = text;
			}

			public object Object { get; }
			public string Text { get; }
		}
	}
}
using Xunit;

namespace Maestro.Tests
{
	public class TodoAttribute : FactAttribute
	{
		public TodoAttribute()
		{
			Skip = "todo";
		}
	}
}
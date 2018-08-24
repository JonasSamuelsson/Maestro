using Xunit;

namespace Maestro.Tests.OldTests
{
	public class TodoAttribute : FactAttribute
	{
		public TodoAttribute()
		{
			Skip = "todo";
		}
	}
}
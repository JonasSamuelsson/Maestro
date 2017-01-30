using System;
using System.Linq;

namespace Maestro.Tests
{
	public class type_listings
	{
		public void list_types()
		{
			var categories = from t in typeof(Container).Assembly.GetTypes()
								  where t.IsPublic
								  orderby t.FullName
								  let category = t.IsClass
									  ? "class"
									  : t.IsEnum
										  ? "enum"
										  : t.IsInterface
											  ? "interface"
											  : "other"
								  group t by category into g
								  orderby g.Key
								  select g;

			foreach (var category in categories)
			{
				Console.WriteLine(category.Key);
				foreach (var type in category) Console.WriteLine(type.FullName);
				Console.WriteLine();
			}
		}
	}
}
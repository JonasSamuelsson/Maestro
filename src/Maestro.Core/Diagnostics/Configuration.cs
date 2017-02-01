using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maestro.Diagnostics
{
	public class Configuration
	{
		public IEnumerable<Service> Services { get; internal set; }

		public override string ToString()
		{
			var rows = new List<string[]> { new[] { "Service type", "Name", "Provider", "Lifetime", "Instance type" } };
			foreach (var service in Services)
			{
				rows.Add(new[]
				{
					service.ServiceType.ToString(),
					GetServiceName(service),
					service.Provider,
					service.Lifetime,
					service.InstanceType?.FullName ?? string.Empty
				});
			}

			var lengths = rows.First()
				.Select((_, i) => rows.Max(x => x[i].Length))
				.ToList();

			var builder = new StringBuilder();

			foreach (var row in rows)
			{
				for (var i = 0; i < row.Length; i++)
				{
					builder.Append(row[i].PadRight(lengths[i] + 2));
				}

				builder.AppendLine();
			}

			return builder.ToString();
		}

		private static string GetServiceName(Service service)
		{
			return service.Name == null
				? "{anonymous}"
				: service.Name == string.Empty
					? "{default}"
					: service.Name;
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maestro.Diagnostics
{
	internal class Configuration
	{
		public List<Pipeline> Pipelines { get; } = new List<Pipeline>();
		public List<Service> Services { get; } = new List<Service>();

		public override string ToString()
		{
			var builder = new StringBuilder();

			AddServices(builder);

			builder.AppendLine();
			builder.AppendLine();

			AddPipelines(builder);

			return builder.ToString();
		}

		private void AddServices(StringBuilder builder)
		{
			var lines = from service in Services
							let serviceType = service.ServiceType.ToFormattedString()
							orderby serviceType
							select new[]
							{
					serviceType,
					service.Name,
					service.Id?.ToString() ?? string.Empty,
					service.Provider,
					service.Lifetime,
					service.InstanceType.ToFormattedString()
				};

			var headers = new[] { "Service type", "Name", "Id", "Provider", "Lifetime", "Instance type" };
			foreach (var row in GetOutput("Services", headers, lines))
				builder.AppendLine(row);
		}

		private void AddPipelines(StringBuilder builder)
		{
			var lines = from pipeline in Pipelines
							let serviceType = pipeline.Type.ToFormattedString()
							orderby serviceType
							from service in pipeline.Services
							let isFirst = service == pipeline.Services[0]
							select new[]
							{
					isFirst ? serviceType : string.Empty,
					isFirst ? pipeline.Name : string.Empty,
					service.Id?.ToString() ?? string.Empty,
					service.Provider,
					service.InstanceType.ToFormattedString()
				};

			var headers = new[] { "Service type", "Name", "Id", "Provider", "Instance type" };
			foreach (var row in GetOutput("Pipelines", headers, lines))
				builder.AppendLine(row);
		}

		private static IEnumerable<string> GetOutput(string header, IEnumerable<string> headers, IEnumerable<IEnumerable<string>> data)
		{
			yield return $" {header} ";
			yield return new string('=', header.Length + 2);
			yield return string.Empty;

			var table = new List<string[]> { headers.ToArray() };
			table.AddRange(data.Select(x => x.Select(s => (s ?? string.Empty).Trim()).ToArray()));

			var widths = table
				.First()
				.Select((_, i) => table.Max(x => x[i].Length))
				.ToList();

			foreach (var row in table)
			{
				for (var i = 0; i < row.Length; i++)
				{
					row[i] = $" {row[i].PadRight(widths[i])} ";
				}
			}

			table.Insert(1, widths.Select(width => new string('=', width + 2)).ToArray());

			foreach (var row in table)
				yield return string.Join(" ", row);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maestro.Diagnostics
{
	internal class Configuration
	{
		public Configuration Parent { get; set; }
		public List<Service> Services { get; set; } = new List<Service>();

		public string ToString(Func<Type, bool> serviceFilter)
		{
			var serviceCollections = GetServiceCollections(serviceFilter).ToList();

			serviceCollections.ForEach(x => x.Insert(0, new[] { "Service type", "Name", "Kind", "Lifetime", "Instance type" }));

			var allServices = serviceCollections.SelectMany(x => x).ToList();
			var widths = allServices
				.First()
				.Select((_, i) => allServices.Max(x => x[i].Length))
				.ToList();

			serviceCollections.ForEach(services => services.Insert(1, widths.Select(width => new string('=', width)).ToArray()));

			var builder = new StringBuilder();

			var parent = false;
			foreach (var services in serviceCollections)
			{
				if (parent)
				{
					builder.AppendLine();
					builder.AppendLine(" Parent");
					builder.AppendLine("====================");
					builder.AppendLine();
				}

				foreach (var service in services)
				{
					for (var i = 0; i < service.Length; i++)
					{
						builder.Append(service[i].PadRight(widths[i] + 2));
					}

					builder.AppendLine();
				}

				parent = true;
			}

			return builder.ToString();
		}

		private IEnumerable<List<string[]>> GetServiceCollections(Func<Type, bool> serviceFilter)
		{
			for (var config = this; config != null; config = config.Parent)
			{
				yield return Services
					.Where(service => serviceFilter(service.ServiceType))
					.Select(service => new[]
					{
						service.ServiceType.ToString(),
						GetServiceName(service),
						service.Provider,
						service.Lifetime,
						service.InstanceType?.FullName ?? string.Empty
					}).ToList();
			}
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
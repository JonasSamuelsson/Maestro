using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maestro.Diagnostics
{
	internal class Configuration
	{
		public List<Service> Services { get; set; } = new List<Service>();

		public string ToString(Func<Type, bool> serviceFilter)
		{
			var registeredServices = GetRegisteredServices(serviceFilter).ToList();

			registeredServices.Insert(0, new[] { "Service type", "Name", "Kind", "Lifetime", "Instance type" });


			var widths = registeredServices
				.First()
				.Select((_, i) => registeredServices.Max(x => x[i].Length))
				.ToList();

			registeredServices.Insert(1, widths.Select(width => new string('=', width)).ToArray());

			var builder = new StringBuilder();

			foreach (var service in registeredServices)
			{
				for (var i = 0; i < service.Length; i++)
				{
					builder.Append(service[i].PadRight(widths[i] + 2));
				}

				builder.AppendLine();
			}

			return builder.ToString();
		}

		private IEnumerable<string[]> GetRegisteredServices(Func<Type, bool> serviceFilter)
		{
			return Services
				.Where(service => serviceFilter(service.ServiceType))
				.Select(service => new[]
				{
					service.ServiceType.ToString(),
					GetServiceName(service),
					service.Provider,
					service.Lifetime,
					service.InstanceType?.FullName ?? string.Empty
				});
		}

		private static string GetServiceName(Service service)
		{
			return service.Name == null
				? "{{anonymous}}"
				: service.Name == string.Empty
					? "{{default}}"
					: service.Name;
		}
	}
}
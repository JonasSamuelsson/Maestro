using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Diagnostics;
using Maestro.Utils;

namespace Maestro.Internals
{
	internal class ServiceDescriptorLookup : IDisposable
	{
		private int _counter = 1;
		private readonly ThreadSafeDictionary<Type, ServiceFamily> _serviceFamilies = new ThreadSafeDictionary<Type, ServiceFamily>();

		public bool Add(ServiceDescriptor serviceDescriptor, bool throwIfDuplicate = true)
		{
			if (serviceDescriptor.SortOrder == 0)
			{
				serviceDescriptor.SortOrder = _counter++;
			}

			var serviceFamily = _serviceFamilies.GetOrAdd(serviceDescriptor.Type, type => new ServiceFamily { Type = type });
			var serviceDescriptorName = serviceDescriptor.Name;

			if (serviceDescriptorName == ServiceNames.Anonymous)
			{
				serviceFamily.AnonymousServices.Add(serviceDescriptor);
			}
			else
			{
				try
				{
					serviceFamily.NamedServices.Add(serviceDescriptorName, serviceDescriptor);
				}
				catch (ArgumentException)
				{
					if (throwIfDuplicate)
					{
						var msg = serviceDescriptorName == ServiceNames.Default
							? $"Default service of type '{serviceDescriptor.Type.FullName}' has already been registered."
							: $"Service named '{serviceDescriptorName}' of type '{serviceDescriptor.Type.FullName}' has already been registered.";
						throw new DuplicateServiceRegistrationException(msg);
					}
					return false;
				}
			}

			return true;
		}

		public bool TryGetServiceDescriptor(Type type, string name, out ServiceDescriptor serviceDescriptor)
		{
			ServiceFamily serviceFamily;

			if (_serviceFamilies.TryGet(type, out serviceFamily))
			{
				if (serviceFamily.NamedServices.TryGetValue(name, out serviceDescriptor))
				{
					return true;
				}
			}

			Type genericTypeDefinition;
			Type[] genericArguments;
			if (Reflector.IsGeneric(type, out genericTypeDefinition, out genericArguments))
			{
				if (TryGetServiceDescriptor(genericTypeDefinition, name, out serviceDescriptor))
				{
					serviceDescriptor = serviceDescriptor.MakeGeneric(genericArguments);
					Add(serviceDescriptor);
					return true;
				}
			}

			serviceDescriptor = null;
			return false;
		}

		public bool TryGetServiceDescriptors(Type type, out IEnumerable<ServiceDescriptor> serviceDescriptors)
		{
			ServiceFamily serviceFamily;
			var result = new List<ServiceDescriptor>();

			if (_serviceFamilies.TryGet(type, out serviceFamily))
			{
				if (serviceFamily.NamedServices.Count != 0)
				{
					result.AddRange(serviceFamily.NamedServices.Values);
				}

				if (serviceFamily.AnonymousServices.Count != 0)
				{
					result.AddRange(serviceFamily.AnonymousServices);
				}
			}

			Type genericTypeDefinition;
			Type[] genericArguments;
			if (Reflector.IsGeneric(type, out genericTypeDefinition, out genericArguments))
			{
				if (TryGetServiceDescriptors(genericTypeDefinition, out serviceDescriptors))
				{
					serviceDescriptors = serviceDescriptors
						.Where(x => result.All(y => x.CorrelationId != y.CorrelationId))
						.Select(x => x.MakeGeneric(genericArguments))
						.ToList();
					foreach (var serviceDescriptor in serviceDescriptors) Add(serviceDescriptor);
					result.AddRange(serviceDescriptors);
				}
			}

			if (result.Count != 0)
			{
				serviceDescriptors = result;
				return true;
			}

			serviceDescriptors = null;
			return false;
		}

		public void Dispose()
		{
			// todo
		}

		public void Populate(List<Service> services)
		{
			foreach (var serviceFamily in _serviceFamilies.OrderBy(x => x.Key.FullName).Select(x => x.Value))
			{
				var serviceDescriptors = serviceFamily.NamedServices
					.Select(x => x.Value)
					.OrderBy(x => x.Name)
					.Concat(serviceFamily.AnonymousServices);

				foreach (var serviceDescriptor in serviceDescriptors)
				{
					services.Add(new Service
					{
						InstanceType = serviceDescriptor.FactoryProvider.GetInstanceType(),
						Lifetime = serviceDescriptor.Lifetime.ToString(),
						Name = serviceDescriptor.Name,
						Provider = serviceDescriptor.FactoryProvider.ToString(),
						ServiceType = serviceFamily.Type
					});
				}
			}
		}
	}
}